const express = require('express');
const router = express.Router();
const dataStore = require('../models/dataStore');

// Get cart
router.get('/:sessionId', (req, res) => {
  try {
    const cart = dataStore.getCart(req.params.sessionId);
    const cartWithProducts = cart.map(item => {
      const product = dataStore.getProductById(item.productId);
      return {
        ...item,
        product
      };
    });
    res.json(cartWithProducts);
  } catch (error) {
    res.status(500).json({ error: 'Failed to fetch cart' });
  }
});

// Add to cart
router.post('/:sessionId/add', (req, res) => {
  try {
    const { productId, quantity } = req.body;
    const cart = dataStore.addToCart(req.params.sessionId, parseInt(productId), parseInt(quantity) || 1);
    res.json(cart);
  } catch (error) {
    res.status(500).json({ error: 'Failed to add to cart' });
  }
});

// Remove from cart
router.delete('/:sessionId/remove/:productId', (req, res) => {
  try {
    const cart = dataStore.removeFromCart(req.params.sessionId, parseInt(req.params.productId));
    res.json(cart);
  } catch (error) {
    res.status(500).json({ error: 'Failed to remove from cart' });
  }
});

// Update cart item quantity
router.put('/:sessionId/update', (req, res) => {
  try {
    const { productId, quantity } = req.body;
    const cart = dataStore.updateCartItemQuantity(req.params.sessionId, parseInt(productId), parseInt(quantity));
    res.json(cart);
  } catch (error) {
    res.status(500).json({ error: 'Failed to update cart' });
  }
});

// Clear cart
router.delete('/:sessionId', (req, res) => {
  try {
    dataStore.clearCart(req.params.sessionId);
    res.json({ message: 'Cart cleared' });
  } catch (error) {
    res.status(500).json({ error: 'Failed to clear cart' });
  }
});

module.exports = router;
