const express = require('express');
const router = express.Router();
const dataStore = require('../models/dataStore');

// Create order
router.post('/', (req, res) => {
  try {
    const { sessionId, customer } = req.body;
    const cart = dataStore.getCart(sessionId);
    
    if (!cart || cart.length === 0) {
      return res.status(400).json({ error: 'Cart is empty' });
    }

    // Calculate total
    let total = 0;
    const items = cart.map(item => {
      const product = dataStore.getProductById(item.productId);
      const itemTotal = product.price * item.quantity;
      total += itemTotal;
      
      // Update stock
      dataStore.updateProductStock(item.productId, item.quantity);
      
      return {
        productId: item.productId,
        productName: product.name,
        quantity: item.quantity,
        price: product.price,
        total: itemTotal
      };
    });

    const order = dataStore.createOrder({
      customer,
      items,
      total,
      sessionId
    });

    // Clear cart after order
    dataStore.clearCart(sessionId);

    res.json(order);
  } catch (error) {
    res.status(500).json({ error: 'Failed to create order' });
  }
});

// Get order by ID
router.get('/:id', (req, res) => {
  try {
    const order = dataStore.getOrderById(req.params.id);
    if (!order) {
      return res.status(404).json({ error: 'Order not found' });
    }
    res.json(order);
  } catch (error) {
    res.status(500).json({ error: 'Failed to fetch order' });
  }
});

// Get all orders
router.get('/', (req, res) => {
  try {
    const orders = dataStore.getAllOrders();
    res.json(orders);
  } catch (error) {
    res.status(500).json({ error: 'Failed to fetch orders' });
  }
});

module.exports = router;
