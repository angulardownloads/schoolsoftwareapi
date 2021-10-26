#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
#
#FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
#WORKDIR /src
#COPY ["schoolsoftwareapi/schoolsoftwareapi.csproj", "schoolsoftwareapi/"]
#RUN dotnet restore "schoolsoftwareapi/schoolsoftwareapi.csproj"
#COPY . .
#WORKDIR "/src/schoolsoftwareapi"
#RUN dotnet build "schoolsoftwareapi.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "schoolsoftwareapi.csproj" -c Release -o /app/publish
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#CMD dotnet schoolsoftwareapi.dll
#
#



FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
<<<<<<< HEAD
COPY --from=build-env /app/out .
CMD dotnet schoolsoftwareapi.dll
=======
COPY --from=publish /app/publish .
CMD dotnet schoolsoftwareapi.dll
>>>>>>> 0b0c235ae3c35f65b2427c3345eab14ea99b6915
