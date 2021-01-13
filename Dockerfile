FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet build -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=0 /app/out .
ENTRYPOINT ["./DailyPogChamp"]
