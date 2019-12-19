#!/bin/bash

cd src/ClusterBootstrap

echo -e "\033[36m 开始执行 \033[0m"
sh ../../install_prerequisites.sh
echo -e "\033[36m =============== \033[0m"
echo -e "\033[32m 执行完毕 \033[0m"

mv config.yaml.template config.yaml
echo -e "\033[36m =============== \033[0m"
echo -e "\033[32m 执行完毕 \033[0m"

echo -e "\033[36m 开始执行 \033[0m"
./deploy.py -y build
wget http://ccsdatarepo.westus.cloudapp.azure.com/data/containernetworking/cni-amd64-v0.5.2.tgz
mv cni-amd64-v0.5.2.tgz ./deploy/bin/
echo -e "\033[36m =============== \033[0m"
echo -e "\033[32m 执行完毕 \033[0m"

echo -e "\033[36m 开始执行 \033[0m"
echo "输入部署的用户名："
read input
echo $input | sudo tee ./deploy/sshkey/rootuser
echo "输入部署的用户名密码："
read input
echo $input | sudo tee ./deploy/sshkey/rootpasswd
echo -e "\033[36m =============== \033[0m"
echo -e "\033[32m 执行完毕 \033[0m"

echo -e "\033[36m 开始执行 \033[0m"
./deploy.py sshkey install
./deploy.py execonall sudo ls -al
echo -e "\033[36m =============== \033[0m"
echo -e "\033[32m 执行完毕 \033[0m"

echo -e "\033[36m 开始执行 \033[0m"
./deploy.py runscriptonall ./scripts/prepare_ubuntu.sh
echo -e "\033[36m =============== \033[0m"
echo -e "\033[32m 执行完毕 \033[0m"

echo -e "\033[36m 开始执行 \033[0m"
/deploy.py execonall docker pull dlws/pause-amd64:3.0
./deploy.py execonall docker tag  dlws/pause-amd64:3.0 gcr.io/google_containers/pause-amd64:3.0
echo -e "\033[36m =============== \033[0m"
echo -e "\033[32m 执行完毕 \033[0m"

echo -e "\033[36m 开始执行 \033[0m"
./deploy.py -y deploy
./deploy.py -y updateworker
./deploy.py -y kubernetes labels
./deploy.py -y kubernetes uncordon
sudo ln -s /home/ubuntu/QianJiangYuan/src/ClusterBootstrap/deploy/bin/kubectl /usr/bin/kubectl
echo -e "\033[36m =============== \033[0m"
echo -e "\033[32m 执行完毕 \033[0m"

echo -e "\033[36m 开始执行 \033[0m"
./deploy.py mount
./deploy.py kubectl create -f https://raw.githubusercontent.com/NVIDIA/k8s-device-plugin/v1.9/nvidia-device-plugin.yml
echo -e "\033[36m =============== \033[0m"
echo -e "\033[32m 执行完毕 \033[0m"
