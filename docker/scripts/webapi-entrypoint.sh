if [ -f /etc/secrets/app-settings.json ]; then
  cp /etc/secrets/app-settings.json /app
else
  echo "No app-settings.json config specified."
fi

dotnet ideas-server.dll "urls=http://0.0.0.0:5000"