.PHONY: up up-min down kafka logs clean

up:
	@echo " Starting all services (full)..."
	docker compose -f docker-compose.yml --profile full up -d

up-min:
	@echo " Starting minimal services (postgres, mongo, kafka)..."
	docker compose -f docker-compose.yml --profile minimal up -d

down:
	@echo " Stopping containers..."
	docker compose -f docker-compose.yml down

kafka:
	@echo " Starting Kafka only..."
	docker compose -f docker-compose.yml up -d kafka

logs:
	@docker compose logs -f

clean:
	@echo " Removing all volumes..."
	docker compose down -v
