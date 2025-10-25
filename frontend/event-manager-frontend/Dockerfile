FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build:ssr

FROM node:20-alpine AS runtime
WORKDIR /app
COPY --from=build /app/dist/event-manager-frontend ./dist
COPY --from=build /app/node_modules ./node_modules
COPY --from=build /app/package.json ./
EXPOSE 4000
RUN ls -la dist/server
CMD ["node", "dist/server/server.mjs"]
