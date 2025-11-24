#!/usr/bin/env node

/**
 * MCP Server for API Testing
 * Provides tools to test Coffee Restaurant API endpoints
 */

import { Server } from '@modelcontextprotocol/sdk/server/index.js';
import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';
import {
  CallToolRequestSchema,
  ListToolsRequestSchema,
} from '@modelcontextprotocol/sdk/types.js';
import axios from 'axios';

// API configuration from environment
const apiBaseUrl = process.env.API_BASE_URL || 'https://localhost:7001';
const apiTimeout = parseInt(process.env.API_TIMEOUT || '30000');

// Create axios instance
const apiClient = axios.create({
  baseURL: apiBaseUrl,
  timeout: apiTimeout,
  headers: {
    'Content-Type': 'application/json',
  },
  // Allow self-signed certificates in development
  httpsAgent: new (await import('https')).Agent({
    rejectUnauthorized: false,
  }),
});

// JWT token storage
let authToken = null;

// Create MCP server
const server = new Server(
  {
    name: 'coffee-restaurant-api-tester',
    version: '1.0.0',
  },
  {
    capabilities: {
      tools: {},
    },
  }
);

// Define available tools
server.setRequestHandler(ListToolsRequestSchema, async () => {
  return {
    tools: [
      {
        name: 'api_login',
        description: 'Login to the API and store JWT token for subsequent requests',
        inputSchema: {
          type: 'object',
          properties: {
            email: {
              type: 'string',
              description: 'User email (default: admin@coffeerestaurant.com)',
            },
            password: {
              type: 'string',
              description: 'User password (default: Admin123!)',
            },
          },
        },
      },
      {
        name: 'api_register',
        description: 'Register a new user account',
        inputSchema: {
          type: 'object',
          properties: {
            email: {
              type: 'string',
              description: 'User email',
            },
            password: {
              type: 'string',
              description: 'User password',
            },
            fullName: {
              type: 'string',
              description: 'User full name',
            },
            role: {
              type: 'string',
              enum: ['Customer', 'Barista', 'Admin'],
              description: 'User role (default: Customer)',
            },
          },
          required: ['email', 'password', 'fullName'],
        },
      },
      {
        name: 'api_get_coffee_items',
        description: 'Get all available coffee items',
        inputSchema: {
          type: 'object',
          properties: {},
        },
      },
      {
        name: 'api_create_coffee_item',
        description: 'Create a new coffee item (requires Admin role)',
        inputSchema: {
          type: 'object',
          properties: {
            name: {
              type: 'string',
              description: 'Coffee item name',
            },
            description: {
              type: 'string',
              description: 'Coffee item description',
            },
            price: {
              type: 'number',
              description: 'Coffee item price',
            },
            categoryId: {
              type: 'string',
              description: 'Category GUID',
            },
            isAvailable: {
              type: 'boolean',
              description: 'Availability status (default: true)',
            },
          },
          required: ['name', 'description', 'price', 'categoryId'],
        },
      },
      {
        name: 'api_get_orders',
        description: 'Get all orders (requires authentication)',
        inputSchema: {
          type: 'object',
          properties: {},
        },
      },
      {
        name: 'api_create_order',
        description: 'Create a new order (requires authentication)',
        inputSchema: {
          type: 'object',
          properties: {
            customerId: {
              type: 'string',
              description: 'Customer GUID',
            },
            baristaId: {
              type: 'string',
              description: 'Barista GUID (optional)',
            },
            orderItems: {
              type: 'array',
              description: 'Array of order items',
              items: {
                type: 'object',
                properties: {
                  coffeeItemId: {
                    type: 'string',
                    description: 'Coffee item GUID',
                  },
                  quantity: {
                    type: 'number',
                    description: 'Quantity',
                  },
                },
                required: ['coffeeItemId', 'quantity'],
              },
            },
          },
          required: ['customerId', 'orderItems'],
        },
      },
      {
        name: 'api_health_check',
        description: 'Check if the API is running and responsive',
        inputSchema: {
          type: 'object',
          properties: {},
        },
      },
      {
        name: 'api_custom_request',
        description: 'Make a custom HTTP request to any endpoint',
        inputSchema: {
          type: 'object',
          properties: {
            method: {
              type: 'string',
              enum: ['GET', 'POST', 'PUT', 'PATCH', 'DELETE'],
              description: 'HTTP method',
            },
            endpoint: {
              type: 'string',
              description: 'API endpoint path (e.g., /api/coffeeitems)',
            },
            body: {
              type: 'object',
              description: 'Request body (for POST/PUT/PATCH)',
            },
            requiresAuth: {
              type: 'boolean',
              description: 'Whether to include JWT token (default: true)',
            },
          },
          required: ['method', 'endpoint'],
        },
      },
    ],
  };
});

// Handle tool calls
server.setRequestHandler(CallToolRequestSchema, async (request) => {
  const { name, arguments: args } = request.params;

  try {
    switch (name) {
      case 'api_login': {
        const email = args.email || 'admin@coffeerestaurant.com';
        const password = args.password || 'Admin123!';

        const response = await apiClient.post('/api/auth/login', {
          email,
          password,
        });

        if (response.data.success && response.data.data.token) {
          authToken = response.data.data.token;
          apiClient.defaults.headers.common['Authorization'] = `Bearer ${authToken}`;

          return {
            content: [
              {
                type: 'text',
                text: JSON.stringify(
                  {
                    success: true,
                    message: 'Login successful',
                    user: response.data.data.user,
                    tokenStored: true,
                  },
                  null,
                  2
                ),
              },
            ],
          };
        } else {
          throw new Error('Login failed: No token received');
        }
      }

      case 'api_register': {
        const response = await apiClient.post('/api/auth/register', {
          email: args.email,
          password: args.password,
          fullName: args.fullName,
          role: args.role || 'Customer',
        });

        return {
          content: [
            {
              type: 'text',
              text: JSON.stringify(response.data, null, 2),
            },
          ],
        };
      }

      case 'api_get_coffee_items': {
        const response = await apiClient.get('/api/coffeeitems');

        return {
          content: [
            {
              type: 'text',
              text: JSON.stringify(response.data, null, 2),
            },
          ],
        };
      }

      case 'api_create_coffee_item': {
        if (!authToken) {
          throw new Error('Not authenticated. Please use api_login first.');
        }

        const response = await apiClient.post('/api/coffeeitems', {
          name: args.name,
          description: args.description,
          price: args.price,
          categoryId: args.categoryId,
          isAvailable: args.isAvailable !== undefined ? args.isAvailable : true,
        });

        return {
          content: [
            {
              type: 'text',
              text: JSON.stringify(response.data, null, 2),
            },
          ],
        };
      }

      case 'api_get_orders': {
        if (!authToken) {
          throw new Error('Not authenticated. Please use api_login first.');
        }

        const response = await apiClient.get('/api/orders');

        return {
          content: [
            {
              type: 'text',
              text: JSON.stringify(response.data, null, 2),
            },
          ],
        };
      }

      case 'api_create_order': {
        if (!authToken) {
          throw new Error('Not authenticated. Please use api_login first.');
        }

        const response = await apiClient.post('/api/orders', {
          customerId: args.customerId,
          baristaId: args.baristaId,
          orderItems: args.orderItems,
        });

        return {
          content: [
            {
              type: 'text',
              text: JSON.stringify(response.data, null, 2),
            },
          ],
        };
      }

      case 'api_health_check': {
        try {
          const response = await apiClient.get('/api/coffeeitems');
          return {
            content: [
              {
                type: 'text',
                text: JSON.stringify(
                  {
                    status: 'healthy',
                    statusCode: response.status,
                    message: 'API is running and responsive',
                  },
                  null,
                  2
                ),
              },
            ],
          };
        } catch (error) {
          return {
            content: [
              {
                type: 'text',
                text: JSON.stringify(
                  {
                    status: 'unhealthy',
                    error: error.message,
                  },
                  null,
                  2
                ),
              },
            ],
          };
        }
      }

      case 'api_custom_request': {
        const requiresAuth = args.requiresAuth !== false;

        if (requiresAuth && !authToken) {
          throw new Error('Not authenticated. Please use api_login first or set requiresAuth to false.');
        }

        const config = {
          method: args.method.toLowerCase(),
          url: args.endpoint,
        };

        if (args.body && ['post', 'put', 'patch'].includes(config.method)) {
          config.data = args.body;
        }

        const response = await apiClient.request(config);

        return {
          content: [
            {
              type: 'text',
              text: JSON.stringify(
                {
                  status: response.status,
                  statusText: response.statusText,
                  data: response.data,
                },
                null,
                2
              ),
            },
          ],
        };
      }

      default:
        throw new Error(`Unknown tool: ${name}`);
    }
  } catch (error) {
    return {
      content: [
        {
          type: 'text',
          text: JSON.stringify(
            {
              error: error.message,
              response: error.response?.data,
              status: error.response?.status,
            },
            null,
            2
          ),
        },
      ],
      isError: true,
    };
  }
});

// Start server
async function main() {
  const transport = new StdioServerTransport();
  await server.connect(transport);
  console.error('Coffee Restaurant API Tester MCP server running on stdio');
}

main().catch((error) => {
  console.error('Server error:', error);
  process.exit(1);
});
