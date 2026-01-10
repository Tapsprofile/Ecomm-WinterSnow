// Session management
function getSessionId() {
    let sessionId = localStorage.getItem('sessionId');
    if (!sessionId) {
        sessionId = 'session_' + Date.now() + '_' + Math.random().toString(36).substring(2, 11);
        localStorage.setItem('sessionId', sessionId);
    }
    return sessionId;
}

// API calls
const API_URL = '';

async function fetchProducts() {
    try {
        const response = await fetch(`${API_URL}/api/products`);
        return await response.json();
    } catch (error) {
        console.error('Error fetching products:', error);
        return [];
    }
}

async function addToCart(productId, quantity = 1) {
    try {
        const sessionId = getSessionId();
        const response = await fetch(`${API_URL}/api/cart/${sessionId}/add`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ productId, quantity })
        });
        return await response.json();
    } catch (error) {
        console.error('Error adding to cart:', error);
        return null;
    }
}

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

// Update cart count in navbar
async function updateCartCount() {
    const cart = await getCart();
    const totalItems = cart.reduce((sum, item) => sum + item.quantity, 0);
    const cartCountElement = document.getElementById('cart-count');
    if (cartCountElement) {
        cartCountElement.textContent = totalItems;
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

// Display products
async function displayProducts() {
    const products = await fetchProducts();
    const productsGrid = document.getElementById('products-grid');
    
    if (!productsGrid) return;

    productsGrid.innerHTML = products.map(product => `
        <div class="product-card">
            <div class="product-image">
                ${productIcons[product.name] || '📦'}
            </div>
            <div class="product-info">
                <div class="product-category">${product.category}</div>
                <h3 class="product-name">${product.name}</h3>
                <p class="product-description">${product.description}</p>
                <div class="product-footer">
                    <span class="product-price">$${product.price.toFixed(2)}</span>
                    <span class="product-stock">Stock: ${product.stock}</span>
                </div>
                <button 
                    class="btn" 
                    onclick="handleAddToCart(${product.id}, event)"
                    ${product.stock === 0 ? 'disabled' : ''}
                >
                    ${product.stock === 0 ? 'Out of Stock' : 'Add to Cart'}
                </button>
            </div>
        </div>
    `).join('');
}

// Handle add to cart
async function handleAddToCart(productId, event) {
    await addToCart(productId, 1);
    await updateCartCount();
    
    // Show feedback
    if (event && event.target) {
        const button = event.target;
        const originalText = button.textContent;
        button.textContent = 'Added!';
        button.style.background = '#51cf66';
        
        setTimeout(() => {
            button.textContent = originalText;
            button.style.background = '';
        }, 1500);
    }
}

// Initialize page
document.addEventListener('DOMContentLoaded', () => {
    displayProducts();
    updateCartCount();
});
