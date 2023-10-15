 

start dapr run --app-id weather-api --dapr-http-port 5201 --app-port 5200 --resources-path "./components" -- dotnet run --project "./ServiceInvocation.Api/ServiceInvocation.Api.csproj"
start dapr run --app-id weather-ui --dapr-http-port 5138 --app-port 5137 --resources-path "./components"  -- dotnet run --project "./ServiceInvocation.UI/ServiceInvocation.UI.csproj"

 

 