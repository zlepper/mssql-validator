FROM mcr.microsoft.com/dotnet/sdk:5.0 as build
WORKDIR /source

COPY Validator/. .
RUN sh publish.sh

FROM mcr.microsoft.com/mssql/server:2019-latest
ENV SA_PASSWORD=Passw0rd
ENV ACCEPT_EULA=y

COPY --from=build /app /app
COPY run.sh /app/run.sh
ENTRYPOINT ["bash", "/app/run.sh"]
CMD ""
