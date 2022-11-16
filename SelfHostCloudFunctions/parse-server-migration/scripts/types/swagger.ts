export type SwaggerSchema = {
  type?: string;
  $ref?: string;
};

export interface SwaggerResponse {
  description: string;
  content: unknown;
}

export interface SwaggerRequestBody {
  description: string;
  required: boolean;
  content: {
    'application/json'?: {
      schema: SwaggerSchema;
    };
  };
}

export interface SwaggerPath {
  tags: string[];
  requestBody: SwaggerRequestBody;
  description: string;
  parameters: unknown[];
  summary: string;
  operationId: string;
  responses: {
    200?: SwaggerResponse;
  };
}
export interface Swagger {
  paths: SwaggerPath[];
  components: {
    schemas: Record<
      string,
      {
        properties: Record<string, unknown>;
        required?: string[];
        type?: string;
      }
    >;
  };
}

export interface PathDetails {
  method: string;
  pathName: string;
  data: SwaggerPath;
}
