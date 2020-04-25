cd server
docker build -t serverchat .
docker run -ti --rm -p 3000:80 --name servercontainer serverchat