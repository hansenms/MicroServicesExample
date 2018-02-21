FROM microsoft/dotnet:2.0.5-sdk-2.1.4-jessie

WORKDIR /app
EXPOSE 5000
COPY ./publish .
ENTRYPOINT ["dotnet", "historian.dll"]