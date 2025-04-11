## Test Azure Function for optimizing calling downstream API

Use the Azure Developer CLI to create the infrastructure and deploy the Azure Function to your subscription. 
To build the container, you'll need to have docker configured locally and have the Azure CLI installed. 

You'll need to create an environment name first - this will be used to create the resource group and other resources.

```azd up```

Use the same command if you want to redeploy deploy the function again after making any changes.

### Example query to extract the JSON log fields from the ContainerAppConsoleLogs_CL table

```
ContainerAppConsoleLogs_CL 
| extend log_json = parse_json(Log_s)
| extend Message = iff(isnull(log_json), "", tostring(log_json.Message))
| extend Category = iff(isnull(log_json), "", tostring(log_json.Category))
| extend LogLevel = iff(isnull(log_json), "", tostring(log_json.LogLevel))
| project TimeGenerated, log_json.Timestamp, LogLevel, Category, Message, Log_s
| where Message != ""
```

Running the JMeter test plan from the command line: using the IP Address of the Application Gateway that 
was configured during the build.

```
jmeter -n -t "Test Weather.jmx" -J HOSTNAME=xxx.xxx.xxx.xxx -f -l WeatherResult
```

The resulting output file will log the results of the test run.

