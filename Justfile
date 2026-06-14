export PATH := "/snap/bin:" + env_var("PATH")
export DOTNET_ROOT := "/snap/dotnet-sdk-100/current"

coverage:
    dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage

report: coverage
    dotnet tool run reportgenerator -- -reports:./coverage/**/coverage.cobertura.xml -targetdir:./coverage/report -reporttypes:Html
    xdg-open ./coverage/report/index.html
