/* eslint-disable @typescript-eslint/no-var-requires */
// @ts-ignore
import { RedisCacheAdapter } from 'parse-server';
import config from './config';

const redisClient = new RedisCacheAdapter({ url: process.env.REDIS_CONNECTION_STRING });

const getRateLimitKeys = (identifier: string) => `ratelimit_${identifier}`;

const resetTtl = async (key: string) => {
  await redisClient.put(key, 0, config.RATE_LIMIT_TTL * 1000);
  return 0;
};

const updateRecord = (key: string) => {
  redisClient.client.incr(key);
};

const redisQuery = async (key: string) => {
  return new Promise((resolve) => {
    redisClient.client.ttl(key, async (error: Error, data: any) => {
      if (data < 0) {
        resolve(resetTtl(key));
      }
      const result = await redisClient.get(key);
      resolve(result);
    });
  });
};

const checkStatus = async (identifier: string, requestLimit: number) => {
  const key = getRateLimitKeys(identifier);
  const rateLimitCount = (await redisQuery(key)) as number;
  let response;
  if (rateLimitCount < requestLimit) {
    updateRecord(key);
    response = true;
  } else {
    response = false;
  }
  return response;
};

export const handleRateLimit = async (user: any, ip: any) => {
  let status;
  if (user && user.id) {
    status = await checkStatus(user.id, config.RATE_LIMIT_AUTHENTICATED);
  }
  status = await checkStatus(ip, config.RATE_LIMIT_ANONYMOUS);

  return status;
};
