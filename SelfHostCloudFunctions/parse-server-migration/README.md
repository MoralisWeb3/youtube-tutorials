# parse-server migration

This demo project contains a parse-server backend to migrate from a hosted server on Moralis to a self-hosted server, using parse-server.

## Getting started locally

1. Copy/download this project
2. Make sure to have `yarn` or `npm` insalled
3. Setup mongo-db and redis locally (see below)
5. Install all dependencies via `yarn install` or `npm install` 
6. Copy `.env.example` to `.env` and fill in the values

### Run your dapp

- Run `yarn dev` to run the server locally

Now your app is running locally on `localhost:1337/server` (or any other port/endpoint you set in `.env`)

Note: by default the cloud-code is referenced in build/cloud, so make sure to run `yarn build` before running the server. Or change the location of the cloud code.

## Run mongo-db

In order to run a server instance of parse-server, you will need to setup a mongo-db instance. For more information you can see https://www.mongodb.com/docs/manual/installation/

For local development, you can use the mongo-db-runner (see https://github.com/mongodb-js/runner). **This should only be used for local development**. To start this run:
```
yarn dev:db-start
```
And to stop it, run
```
yarn dev:db-stop
```

Make sure to set the `DATABASE_URI` in your `.env` file

## Run redis

For rate-limiting, we are using a redis instance. In order for this to work, you will need to setup redis instance. For more information you can see https://redis.io/docs/getting-started/

For local development you will need to install redis on your local machine, and start the service. Make sure to set the `REDIS_CONNECTION_STRING` in your `.env` file
