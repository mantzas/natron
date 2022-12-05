DOCKER = docker

default: test

test: 
	dotnet test --filter Category=Unit

testint: 
	dotnet test --filter Category=Integration

testall: 
	dotnet test

deps-start:
	docker-compose up -d

deps-stop:
	docker-compose down

# disallow any parallelism (-j) for Make. This is necessary since some
# commands during the build process create temporary files that collide
# under parallel conditions.
.NOTPARALLEL:

.PHONY: default test testint testall deps-start deps-stop
