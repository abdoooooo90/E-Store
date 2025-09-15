function updateQuantity(cartItemId, newQuantity) {
    if (newQuantity < 1) {
        removeFromCart(cartItemId);
        return;
    }
    
    $.ajax({
        url: '/Cart/UpdateQuantity',
        type: 'POST',
        data: {
            id: cartItemId,
            quantity: newQuantity
        },
        success: function(response) {
            if (response.success) {
                if (typeof updateCartCount === 'function') {
                    try { updateCartCount(); } catch (_) { /* ignore */ }
                }
                location.reload();
            } else {
                alert('Error updating cart: ' + response.message);
            }
        },
        error: function() {
            alert('Error updating cart');
        }
    });
}

function removeFromCart(cartItemId) {
    if (confirm('Are you sure you want to remove this item from your cart?')) {
        $.ajax({
            url: '/Cart/Remove',
            type: 'POST',
            data: {
                id: cartItemId
            },
            success: function(response) {
                if (response.success) {
                    if (typeof updateCartCount === 'function') {
                        try { updateCartCount(); } catch (_) { /* ignore */ }
                    }
                    location.reload();
                } else {
                    alert('Error removing item: ' + response.message);
                }
            },
            error: function() {
                alert('Error removing item from cart');
            }
        });
    }
}
