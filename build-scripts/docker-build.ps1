Set-Location -Path ..\

docker stop co

# remove all stopped containers
docker rm $(docker ps -a -q)

# remove image if already exists 
docker rmi co_img

# build images
docker build -t co_img .

# delete all temp images
docker image prune -f

Set-Location -Path .\build-scripts