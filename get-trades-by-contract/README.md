# Using Moralis NFT API with Django and React.

This is simple implementation for Moralis NFT Api using Django and React in order to get NFT Trades of a contract.

## Prerequisites

1. Install Django and rest framework.
   
   ```Shell
        pip install django
        pip install djangorestframework django-cors-headers
   ```
2. Install moralis

   ```bash
       pip install moralis 
   ```
3. On frontend folder install the npm dependencies.

   ```bash
       npm install 
   ```

## Usage

1. Set up your Moralis API key on `backend/nft/.env`.
2. Run the Django server on backend with:
   
   ```Shell
        python manage.py runserver
   ```
3. Run the frontend React server on frontend folder run:

   ```Shell
       npm start 
   ```
4. Interact with the application.