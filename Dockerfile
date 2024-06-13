FROM my-custom-dotnet-sdk:8.0-preview AS installer-env

# Use alternative Debian mirrors
RUN sed -i 's|http://deb.debian.org/debian|http://ftp.de.debian.org/debian|g' /etc/apt/sources.list && \
    sed -i 's|http://deb.debian.org/debian-security|http://security.debian.org/debian-security|g' /etc/apt/sources.list

# Update package sources and install dependencies
RUN apt-get update && \
    apt-get install -y curl apt-transport-https unixodbc-dev libsasl2-modules-gssapi-mit libltdl-dev

# Copy the local Simba Spark ODBC Driver .deb file into the image
COPY simbaspark_2.8.0.1002-2_amd64.deb /tmp/simbaspark_2.8.0.1002-2_amd64.deb
RUN mkdir /usr/local/odbc/
COPY ./ini/odbc/* /usr/local/odbc/
COPY ./ini/libltdl.so.7 /home/site/wwwroot/runtimes/linux-x64/native/
COPY ./ini/simba.sparkodbc.ini /etc/


# Install dependencies required by Simba Spark ODBC Driver
RUN apt-get update && \
    apt-get install -y libsasl2-modules-gssapi-mit

# Install the Simba Spark ODBC Driver
RUN dpkg -i /tmp/simbaspark_2.8.0.1002-2_amd64.deb && \
    apt-get install -f -y && \
    rm /tmp/simbaspark_2.8.0.1002-2_amd64.deb

# Set environment variables for ODBC
ENV ODBCINI=/usr/local/odbc/odbc.ini
ENV ODBCSYSINI=/usr/local/odbc/
ENV SPARKINI=/etc/simba.sparkodbc.ini

# -----------------------------------------------------
# IMPORTANT: Copy and link libraries BEFORE the build step
# -----------------------------------------------------

# Copy and link libraries to the final image location
RUN mkdir -p /home/site/wwwroot/runtimes/linux-x64/native
RUN cp /lib/x86_64-linux-gnu/libodbc.so.2 /home/site/wwwroot/runtimes/linux-x64/native/


# Copy and build your application (moved after library setup)
COPY . /src/dotnet-function-app
WORKDIR /src/dotnet-function-app
RUN mkdir -p /home/site/wwwroot && \
    dotnet publish *.csproj --output /home/site/wwwroot

# Final Stage (Azure Functions Runtime)
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0

ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true
ENV LD_LIBRARY_PATH=/usr/local/lib:$LD_LIBRARY_PATH:/home/site/wwwroot/runtimes/linux-x64/native/:/usr/share/dotnet/shared/Microsoft.NETCore.App/8.0.6/:/home/site/wwwroot/runtimes/linux/lib/net8.0/:/home/site/wwwroot/runtimes/linux-x64/native/:/usr/share/dotnet/shared/Microsoft.NETCore.App/8.0.6/:/home/site/wwwroot/runtimes/linux/lib/net8.0/:/home/site/wwwroot/runtimes/linux-x64/native/:/usr/share/dotnet/shared/Microsoft.NETCore.App/8.0.6/:/home/site/wwwroot/runtimes/linux/lib/net8.0/

COPY --from=installer-env /home/site/wwwroot /home/site/wwwroot

COPY ./ini/libltdl.so.7 /usr/share/dotnet/shared/Microsoft.NETCore.App/8.0.6/
COPY ./ini/libltdl.so.7 /home/site/wwwroot/runtimes/linux/lib/net8.0/
COPY ./ini/libltdl.so.7 /home/site/wwwroot/runtimes/linux-x64/native/
COPY ./ini/libltdl.so.7 /usr/share/dotnet/shared/Microsoft.NETCore.App/8.0.6/
COPY ./ini/libltdl.so.7 /home/site/wwwroot/runtimes/linux/lib/net8.0/
COPY ./ini/libltdl.so.7 /home/site/wwwroot/runtimes/linux-x64/native/
COPY ./ini/libltdl.so.7 /usr/share/dotnet/shared/Microsoft.NETCore.App/8.0.6/
COPY ./ini/libltdl.so.7 /home/site/wwwroot/runtimes/linux/lib/net8.0/
COPY ./ini/libltdl.so.7 /home/site/wwwroot/runtimes/linux-x64/native/
COPY ./ini/libltdl.so.7 /usr/share/dotnet/shared/Microsoft.NETCore.App/8.0.6/
COPY ./ini/libltdl.so.7 /home/site/wwwroot/runtimes/linux/lib/net8.0/

COPY ./ini/libodbc.so.2 /usr/share/dotnet/shared/Microsoft.NETCore.App/8.0.6/
COPY ./ini/libodbc.so.2 /home/site/wwwroot/runtimes/linux/lib/net8.0/
COPY ./ini/libodbc.so.2 /home/site/wwwroot/runtimes/linux-x64/native/
COPY ./ini/libodbc.so.2 /usr/share/dotnet/shared/Microsoft.NETCore.App/8.0.6/
COPY ./ini/libodbc.so.2 /home/site/wwwroot/runtimes/linux/lib/net8.0/
COPY ./ini/libodbc.so.2 /home/site/wwwroot/runtimes/linux-x64/native/
COPY ./ini/libodbc.so.2 /usr/share/dotnet/shared/Microsoft.NETCore.App/8.0.6/
COPY ./ini/libodbc.so.2 /home/site/wwwroot/runtimes/linux/lib/net8.0/
COPY ./ini/libodbc.so.2 /home/site/wwwroot/runtimes/linux-x64/native/
COPY ./ini/libodbc.so.2 /usr/share/dotnet/shared/Microsoft.NETCore.App/8.0.6/
COPY ./ini/libodbc.so.2 /home/site/wwwroot/runtimes/linux/lib/net8.0/

# Use alternative Debian mirrors

# Update package sources and install dependencies
RUN apt-get update && \
    apt-get install -y curl apt-transport-https unixodbc-dev libsasl2-modules-gssapi-mit libltdl-dev

# Copy the local Simba Spark ODBC Driver .deb file into the image
COPY simbaspark_2.8.0.1002-2_amd64.deb /tmp/simbaspark_2.8.0.1002-2_amd64.deb
RUN mkdir /usr/local/odbc/
COPY ./ini/odbc/* /usr/local/odbc/
COPY ./ini/libltdl.so.7 /home/site/wwwroot/runtimes/linux-x64/native/
COPY ./ini/simba.sparkodbc.ini /etc/


# Install dependencies required by Simba Spark ODBC Driver
RUN apt-get update && \
    apt-get install -y libsasl2-modules-gssapi-mit

# Install the Simba Spark ODBC Driver
RUN dpkg -i /tmp/simbaspark_2.8.0.1002-2_amd64.deb && \
    apt-get install -f -y && \
    rm /tmp/simbaspark_2.8.0.1002-2_amd64.deb

# Set environment variables for ODBC
ENV ODBCINI=/usr/local/odbc/odbc.ini
ENV ODBCSYSINI=/usr/local/odbc/
ENV SPARKINI=/etc/simba.sparkodbc.ini
