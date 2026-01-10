# WinterSnow E-commerce Platform

A modern, full-stack e-commerce platform for winter clothing and accessories.

## Features

- 🛍️ Product browsing and catalog
- 🛒 Shopping cart functionality
- 💳 Checkout process
- 📦 Order management
- 🎨 Modern, responsive UI
- 🔒 Basic authentication support

## Tech Stack

- **Backend**: Node.js, Express.js
- **Frontend**: HTML, CSS, JavaScript (Vanilla)
- **Data Storage**: In-memory (for demo purposes)

## Installation

1. Clone the repository:
```bash
git clone https://github.com/Tapsprofile/Ecomm-WinterSnow.git
cd Ecomm-WinterSnow
```

2. Install dependencies:
```bash
npm install
```

3. Create environment file:
```bash
cp .env.example .env
```

4. Start the server:
```bash
npm start
```

Or for development with auto-reload:
```bash
npm run dev
```

The application will be available at `http://localhost:3000`

## Project Structure

```
Ecomm-WinterSnow/
├── models/              # Data models and in-memory storage
├── routes/              # API routes
│   ├── auth.js         # Authentication endpoints
│   ├── cart.js         # Shopping cart endpoints
│   ├── orders.js       # Order management endpoints
│   └── products.js     # Product catalog endpoints
├── public/              # Frontend files
│   ├── css/            # Stylesheets
│   ├── js/             # JavaScript files
│   ├── images/         # Product images
│   ├── index.html      # Main product page
│   └── cart.html       # Shopping cart page
├── server.js            # Express server setup
├── package.json         # Dependencies and scripts
└── README.md           # This file
```

## API Endpoints

### Products
- `GET /api/products` - Get all products
- `GET /api/products/:id` - Get product by ID

### Cart
- `GET /api/cart/:sessionId` - Get cart items
- `POST /api/cart/:sessionId/add` - Add item to cart
- `PUT /api/cart/:sessionId/update` - Update cart item quantity
- `DELETE /api/cart/:sessionId/remove/:productId` - Remove item from cart
- `DELETE /api/cart/:sessionId` - Clear cart

### Orders
- `POST /api/orders` - Create new order
- `GET /api/orders/:id` - Get order by ID
- `GET /api/orders` - Get all orders

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login

## Usage

1. Visit the homepage to browse products
2. Click "Add to Cart" to add items to your shopping cart
3. View your cart by clicking the "Cart" link in the navigation
4. Adjust quantities or remove items as needed
5. Fill in the checkout form with your information
6. Click "Place Order" to complete your purchase

## Product Catalog

The platform currently features winter essentials:
- Winter Jackets
- Snow Boots
- Thermal Gloves
- Winter Hats
- Ski Goggles
- Thermal Pants

## Notes

- This is a demo e-commerce platform
- Data is stored in-memory and will be lost when the server restarts
- For production use, integrate with a real database (MongoDB, PostgreSQL, etc.)
- Add proper payment processing integration for real transactions
- Implement proper security measures for authentication and data protection

## License

See LICENSE file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
