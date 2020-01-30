FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build-env
WORKDIR /PaymentGateway
COPY ./ .
WORKDIR /PaymentGateway
RUN dotnet restore --packages ./packages
RUN dotnet publish -c Release -o ./published

FROM mcr.microsoft.com/dotnet/core/sdk:3.1
WORKDIR /PaymentGateway
COPY --from=build-env /PaymentGateway/published .
ENTRYPOINT ["dotnet", "PaymentGateway.Api.dll"]

