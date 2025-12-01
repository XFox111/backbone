FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /build

# Installing additional dev dependencies for AOT
RUN apk add clang binutils musl-dev build-base zlib-static

ADD *.csproj .
RUN dotnet restore --runtime linux-musl-x64

ADD . ./
RUN dotnet publish --configuration Release --no-restore --output /out

FROM scratch AS prod
WORKDIR /app

COPY --from=build /out/Backbone .
COPY --from=build /out/appsettings*.json .

EXPOSE 80
ENTRYPOINT [ "./Backbone" ]
CMD [ "--urls", "http://*:80" ]
