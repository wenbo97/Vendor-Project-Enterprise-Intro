#!/bin/bash

cd /app/backend/publish

# Start backend app
echo "[*] Starting BeverageGalleryBotWebApi on port 5000..."
export ASPNETCORE_ENVIRONMENT=Production

nohup dotnet BeverageGalleryBotWebApi.dll > backend-app.log 2>&1 &
echo $! > backend-app.pid

sleep 1  # Wait for app start...

# Check backend service process
echo "[*] Checking process:"
ps aux | grep '[B]everageGalleryBotWebApi'

# Backend service port check
echo "[*] Checking port 5000:"
sudo lsof -i :5000
