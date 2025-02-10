# Use a specific Node version on Alpine Linux for consistency
FROM node:18-alpine

# Set the working directory inside the container
WORKDIR /usr/src/app

# Copy package.json and package-lock.json (if available) first for caching
COPY package*.json ./

# Install Angular CLI globally
RUN npm install -g @angular/cli

# Install app dependencies
RUN npm install

# Copy the rest of the application source code
COPY . .

# Expose port 4200 so the container's Angular app is reachable
EXPOSE 4200

# Run the Angular development server, listening on all network interfaces
CMD ["ng", "serve", "--host", "0.0.0.0"]
