#!/bin/bash 

which sudo
if [ $? -eq 0 ]
then
    echo "sudo was installed"
else
    apt-get update
    apt-get install sudo
fi

sudo apt-get update 
sudo apt-get update && sudo apt-get install -y --no-install-recommends \
        apt-utils \
        dnsutils \
        software-properties-common \
        build-essential \
        cmake \
        git \
        curl \
        wget \
        protobuf-compiler \
        python-dev \
        python-numpy \
        python-pip \
        cpio \
        mkisofs \
        apt-transport-https \
        openssh-client \
        ca-certificates \
        vim \
        sudo \
        git-all \
        sshpass \
        bison \
        libcurl4-openssl-dev libssl-dev 

curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -

sudo add-apt-repository \
   "deb [arch=amd64] https://download.docker.com/linux/ubuntu \
   $(lsb_release -cs) \
   stable"

sudo apt-key fingerprint 0EBFCD88

# Install docker
echo "Docker Installation .... "
sudo apt-get update
sudo apt-get install docker-ce

echo "PIP installation .... "
sudo pip install --upgrade pip
sudo pip install setuptools 
sudo pip install pyyaml jinja2 flask flask.restful tzlocal pycurl

AZ_REPO=$(lsb_release -cs)
echo "deb [arch=amd64] https://packages.microsoft.com/repos/azure-cli/ $AZ_REPO main" | \
    sudo tee /etc/apt/sources.list.d/azure-cli.list

sudo apt-key --keyring /etc/apt/trusted.gpg.d/Microsoft.gpg adv \
     --keyserver packages.microsoft.com \
     --recv-keys BC528686B50D79E339D3721CEB3E94ADBE1229CF

sudo apt-get install apt-transport-https
sudo apt-get update && sudo apt-get install azure-cli

# Disable Network manager. 
if [ -f /etc/NetworkManager/NetworkManager.conf ]; then
	sed "s/^dns=dnsmasq$/#dns=dnsmasq/" /etc/NetworkManager/NetworkManager.conf > /tmp/NetworkManager.conf && sudo mv /tmp/NetworkManager.conf /etc/NetworkManager/NetworkManager.conf
	sudo service network-manager restart
fi



