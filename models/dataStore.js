// In-memory data store for the application
class DataStore {
  constructor() {
    this.products = [
      {
        id: 1,
        name: 'Winter Jacket',
        description: 'Warm and stylish winter jacket',
        price: 129.99,
        category: 'Outerwear',
        image: '/images/winter-jacket.jpg',
        stock: 25
      },
      {
        id: 2,
        name: 'Snow Boots',
        description: 'Waterproof snow boots for extreme cold',
        price: 89.99,
        category: 'Footwear',
        image: '/images/snow-boots.jpg',
        stock: 40
      },
      {
        id: 3,
        name: 'Thermal Gloves',
        description: 'Insulated thermal gloves',
        price: 24.99,
        category: 'Accessories',
        image: '/images/thermal-gloves.jpg',
        stock: 100
      },
      {
        id: 4,
        name: 'Winter Hat',
        description: 'Cozy knit winter hat',
        price: 19.99,
        category: 'Accessories',
        image: '/images/winter-hat.jpg',
        stock: 75
      },
      {
        id: 5,
        name: 'Ski Goggles',
        description: 'UV protection ski goggles',
        price: 59.99,
        category: 'Accessories',
        image: '/images/ski-goggles.jpg',
        stock: 30
      },
      {
        id: 6,
        name: 'Thermal Pants',
        description: 'Insulated thermal pants for skiing',
        price: 79.99,
        category: 'Outerwear',
        image: '/images/thermal-pants.jpg',
        stock: 35
      }
    ];
    
    this.users = [];
    this.orders = [];
    this.carts = {}; // sessionId: cart items
  }

  // Product methods
  getAllProducts() {
    return this.products;
  }

  getProductById(id) {
    return this.products.find(p => p.id === parseInt(id));
  }

  updateProductStock(id, quantity) {
    const product = this.getProductById(id);
    if (product) {
      product.stock -= quantity;
    }
  }

  // User methods
  createUser(user) {
    const newUser = {
      id: this.users.length + 1,
      ...user,
      createdAt: new Date()
    };
    this.users.push(newUser);
    return newUser;
  }

  getUserByEmail(email) {
    return this.users.find(u => u.email === email);
  }

  getUserById(id) {
    return this.users.find(u => u.id === parseInt(id));
  }

  // Cart methods
  getCart(sessionId) {
    return this.carts[sessionId] || [];
  }

  addToCart(sessionId, productId, quantity = 1) {
    if (!this.carts[sessionId]) {
      this.carts[sessionId] = [];
    }
    
    const existingItem = this.carts[sessionId].find(item => item.productId === productId);
    if (existingItem) {
      existingItem.quantity += quantity;
    } else {
      this.carts[sessionId].push({ productId, quantity });
    }
    
    return this.carts[sessionId];
  }

  removeFromCart(sessionId, productId) {
    if (this.carts[sessionId]) {
      this.carts[sessionId] = this.carts[sessionId].filter(item => item.productId !== productId);
    }
    return this.carts[sessionId];
  }

  updateCartItemQuantity(sessionId, productId, quantity) {
    if (this.carts[sessionId]) {
      const item = this.carts[sessionId].find(item => item.productId === productId);
      if (item) {
        item.quantity = quantity;
      }
    }
    return this.carts[sessionId];
  }

  clearCart(sessionId) {
    this.carts[sessionId] = [];
  }

  // Order methods
  createOrder(order) {
    const newOrder = {
      id: this.orders.length + 1,
      ...order,
      status: 'pending',
      createdAt: new Date()
    };
    this.orders.push(newOrder);
    return newOrder;
  }

  getOrderById(id) {
    return this.orders.find(o => o.id === parseInt(id));
  }

  getOrdersByUserId(userId) {
    return this.orders.filter(o => o.userId === parseInt(userId));
  }

  getAllOrders() {
    return this.orders;
  }
}

// Create singleton instance
const dataStore = new DataStore();

module.exports = dataStore;
