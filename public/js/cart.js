// Session management
function getSessionId() {
    let sessionId = localStorage.getItem('sessionId');
    if (!sessionId) {
        sessionId = 'session_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
        localStorage.setItem('sessionId', sessionId);
    }
    return sessionId;
}

// API calls
const API_URL = '';

async function getCart() {
    try {
        const sessionId = getSessionId();
        const response = await fetch(`${API_URL}/api/cart/${sessionId}`);
        return await response.json();
    } catch (error) {
        console.error('Error fetching cart:', error);
        return [];
    }
}

async function updateCartItem(productId, quantity) {
    try {
        const sessionId = getSessionId();
        const response = await fetch(`${API_URL}/api/cart/${sessionId}/update`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ productId, quantity })
        });
        return await response.json();
    } catch (error) {
        console.error('Error updating cart:', error);
        return null;
    }
}

async function removeFromCart(productId) {
    try {
        const sessionId = getSessionId();
        const response = await fetch(`${API_URL}/api/cart/${sessionId}/remove/${productId}`, {
            method: 'DELETE'
        });
        return await response.json();
    } catch (error) {
        console.error('Error removing from cart:', error);
        return null;
    }
}

async function createOrder(customer) {
    try {
        const sessionId = getSessionId();
        const response = await fetch(`${API_URL}/api/orders`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ sessionId, customer })
        });
        return await response.json();
    } catch (error) {
        console.error('Error creating order:', error);
        return null;
    }
}

// Product icons mapping
const productIcons = {
    'Winter Jacket': '🧥',
    'Snow Boots': '👢',
    'Thermal Gloves': '🧤',
    'Winter Hat': '🎩',
    'Ski Goggles': '🥽',
    'Thermal Pants': '👖'
};

// Update cart count
async function updateCartCount() {
    const cart = await getCart();
    const totalItems = cart.reduce((sum, item) => sum + item.quantity, 0);
    const cartCountElement = document.getElementById('cart-count');
    if (cartCountElement) {
        cartCountElement.textContent = totalItems;
    }
}

// Display cart
async function displayCart() {
    const cart = await getCart();
    const cartContainer = document.getElementById('cart-container');
    const cartSummary = document.getElementById('cart-summary');

    if (!cartContainer || !cartSummary) return;

    if (cart.length === 0) {
        cartContainer.innerHTML = `
            <div class="empty-cart">
                <p>Your cart is empty</p>
                <a href="/" class="btn">Continue Shopping</a>
            </div>
        `;
        cartSummary.innerHTML = '';
        return;
    }

    // Calculate totals
    let subtotal = 0;
    cart.forEach(item => {
        if (item.product) {
            subtotal += item.product.price * item.quantity;
        }
    });
    const tax = subtotal * 0.1; // 10% tax
    const total = subtotal + tax;

    // Display cart items
    cartContainer.innerHTML = `
        <div class="cart-items">
            ${cart.map(item => {
                if (!item.product) return '';
                const itemTotal = item.product.price * item.quantity;
                return `
                    <div class="cart-item">
                        <div class="cart-item-image">
                            ${productIcons[item.product.name] || '📦'}
                        </div>
                        <div class="cart-item-details">
                            <h3>${item.product.name}</h3>
                            <p>${item.product.description}</p>
                            <p><strong>Price:</strong> $${item.product.price.toFixed(2)}</p>
                            <p><strong>Subtotal:</strong> $${itemTotal.toFixed(2)}</p>
                        </div>
                        <div class="cart-item-actions">
                            <div class="quantity-controls">
                                <button class="quantity-btn" onclick="handleUpdateQuantity(${item.productId}, ${item.quantity - 1})">-</button>
                                <span class="quantity-display">${item.quantity}</span>
                                <button class="quantity-btn" onclick="handleUpdateQuantity(${item.productId}, ${item.quantity + 1})">+</button>
                            </div>
                            <button class="remove-btn" onclick="handleRemoveItem(${item.productId})">Remove</button>
                        </div>
                    </div>
                `;
            }).join('')}
        </div>
    `;

    // Display cart summary with checkout form
    cartSummary.innerHTML = `
        <h3>Order Summary</h3>
        <div class="summary-row">
            <span>Subtotal:</span>
            <span>$${subtotal.toFixed(2)}</span>
        </div>
        <div class="summary-row">
            <span>Tax (10%):</span>
            <span>$${tax.toFixed(2)}</span>
        </div>
        <div class="summary-row total">
            <span>Total:</span>
            <span>$${total.toFixed(2)}</span>
        </div>
        <div class="checkout-form">
            <h3>Checkout Information</h3>
            <form onsubmit="handleCheckout(event)">
                <div class="form-group">
                    <label for="name">Full Name</label>
                    <input type="text" id="name" name="name" required>
                </div>
                <div class="form-group">
                    <label for="email">Email</label>
                    <input type="email" id="email" name="email" required>
                </div>
                <div class="form-group">
                    <label for="address">Address</label>
                    <input type="text" id="address" name="address" required>
                </div>
                <div class="form-group">
                    <label for="phone">Phone</label>
                    <input type="tel" id="phone" name="phone" required>
                </div>
                <button type="submit" class="btn btn-success">Place Order</button>
            </form>
        </div>
    `;
}

// Handle quantity update
async function handleUpdateQuantity(productId, newQuantity) {
    if (newQuantity < 1) {
        await handleRemoveItem(productId);
        return;
    }
    await updateCartItem(productId, newQuantity);
    await displayCart();
    await updateCartCount();
}

// Handle remove item
async function handleRemoveItem(productId) {
    if (confirm('Are you sure you want to remove this item?')) {
        await removeFromCart(productId);
        await displayCart();
        await updateCartCount();
    }
}

// Handle checkout
async function handleCheckout(event) {
    event.preventDefault();
    
    const formData = new FormData(event.target);
    const customer = {
        name: formData.get('name'),
        email: formData.get('email'),
        address: formData.get('address'),
        phone: formData.get('phone')
    };

    const order = await createOrder(customer);
    
    if (order) {
        // Show success message
        const cartContainer = document.getElementById('cart-container');
        const cartSummary = document.getElementById('cart-summary');
        
        cartContainer.innerHTML = `
            <div class="success-message">
                <h3>✅ Order Placed Successfully!</h3>
                <p>Order ID: #${order.id}</p>
                <p>Total: $${order.total.toFixed(2)}</p>
                <p>Thank you for your purchase, ${customer.name}!</p>
                <p>A confirmation email will be sent to ${customer.email}</p>
            </div>
            <div style="text-align: center; margin-top: 2rem;">
                <a href="/" class="btn">Continue Shopping</a>
            </div>
        `;
        cartSummary.innerHTML = '';
        
        // Reset cart count
        await updateCartCount();
    } else {
        alert('Failed to place order. Please try again.');
    }
}

// Initialize page
document.addEventListener('DOMContentLoaded', () => {
    displayCart();
    updateCartCount();
});
