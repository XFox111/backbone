# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine-aot AS build
WORKDIR /build

ADD --link . .
RUN --mount=type=cache,target=/root/.nuget \
	--mount=type=cache,target=/build/obj \
	--mount=type=cache,target=/build/bin \
	dotnet publish --runtime linux-musl-x64 --configuration Release --output /out \
		&& rm /out/*.dbg /out/*.endpoints.json /out/appsettings.Development.json

# Production stage
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-alpine AS prod
WORKDIR /app

COPY --link --from=build /out/Backbone .

USER $APP_UID
EXPOSE 8080
ENTRYPOINT [ "./Backbone" ]
