import axios from 'axios';
import { Swagger, SwaggerSchema, PathDetails, SwaggerPath } from '../types/swagger';
import { BodyParams, Endpoint } from '../types/Endpoint';

export const BodyParamTypes = {
  setBody: 'set body',
  property: 'property',
};

const fetchSwaggerJson = async (swaggerUrl: string) => {
  const result = await axios.get(swaggerUrl);
  return result.data as Swagger;
};

const fetchBodyParams = (schema: SwaggerSchema | undefined, swaggerJSON: Swagger, required: boolean) => {
  const body: BodyParams[] = [];

  if (schema?.$ref) {
    const splitSchema = schema.$ref.split('/');
    const schemaKey = splitSchema[splitSchema.length - 1];
    const component = swaggerJSON.components.schemas[schemaKey];
    if (component) {
      Object.entries(component.properties).map(([key]) => {
        return body.push({
          key,
          type: component.type === 'array' ? BodyParamTypes.setBody : BodyParamTypes.property,
          required: (component.required ?? []).includes(key) ? true : false,
        });
      });
    }
  } else {
    body.push({
      key: 'abi',
      type: BodyParamTypes.setBody,
      required,
    });
  }
  return body;
};

const getPathByTag = (swagger: Swagger) => {
  const pathByTag: Record<string, string[]> = {};
  const pathDetails: Record<string, PathDetails> = {};

  Object.entries(swagger.paths).map(([pathName, requestData]) => {
    return Object.entries(requestData).forEach(([method, data]: [string, SwaggerPath]) => {
      const { tags } = data;

      if (tags.length > 0) {
        if (!pathByTag[tags[0]]) {
          pathByTag[tags[0]] = [];
        }
        pathByTag[tags[0]].push(data.operationId);
        pathDetails[data.operationId] = { method, pathName, data };
      }
    });
  });

  return { pathByTag, pathDetails };
};

export const fetchEndpoints = async (swaggerUrl: string) => {
  const swagger = await fetchSwaggerJson(swaggerUrl);
  const { pathDetails, pathByTag } = getPathByTag(swagger);

  const tags = Object.keys(pathByTag);
  const endpoints: Endpoint[] = [];

  Object.keys(pathDetails).forEach((x) => {
    const item = pathDetails[x];

    const endpoint: Endpoint = {
      method: item.method.toUpperCase(),
      group: item.data.tags[0],
      name: x,
      url: item.pathName.split('{').join(':').split('}').join(''),
    };

    if (item.data.requestBody) {
      endpoint.bodyParams = fetchBodyParams(
        item.data.requestBody.content['application/json']?.schema,
        swagger,
        item.data.requestBody.required,
      );
    }

    endpoints.push(endpoint);
  });

  return { endpoints, tags };
};
