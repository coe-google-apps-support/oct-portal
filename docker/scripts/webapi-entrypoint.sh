if [ -f /etc/secrets/appsettings.json ]; then
  cp /etc/secrets/appsettings.json /app
  echo "appsettings.json config specified."
else
  echo "No appsettings.json config specified."
fi

dotnet ideas-server.dll "urls=http://0.0.0.0:5000"