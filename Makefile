# Makefile for HRM microservice dev environment

.PHONY: up down kafka logs clean

up:
	@echo " Starting Kafka and Postgres containers..."
	docker compose -f docker-compose.yml up -d

down:
	@echo " Stopping containers..."
	docker compose -f docker-compose.yml down

kafka:
	@echo " Starting Kafka only..."
	docker compose -f docker-compose.yml up -d kafka

logs:
	@docker compose logs -f

clean:
	@echo "Removing all volumes..."
	docker compose down -v

