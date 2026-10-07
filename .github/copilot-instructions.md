# Copilot Instructions

## Project Guidelines
- Bu projede hiçbir koşulda DateTime.Now kullanılmaz; her zaman DateTime.UtcNow kullanılır.

## Inventory Module Guidelines
- For the Inventory module MVP, prefer a deliberately simple domain model centered on InventoryItem and StockLevel; defer stock reservation entities, movement-history entities, related enums, and their event types until requirements justify them.