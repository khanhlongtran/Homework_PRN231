// imageFetcher.js

function fetchImageWithHeaders(url) {
    fetch(url, {
        method: 'GET',
        headers: {
            'Token': 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJraGFuaGxvbmd0cmFuLmNvbSIsIm5hbWUiOiJLaGFuaCIsImlhdCI6MTUxNjIzOTAyMiwiZXhwIjoyNTE2MjM5MDIyLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjpbIk9yZGVyTWFuYWdlciIsIk1hbmFnZXIiXX0.y8i_V66x3CgaE-UrfhG3N6dNmiz8F9iIHAijIvNpb94',  // Thay 'your-token-here' bằng token thực tế
            'X-Client-Source': 'khanhlongtran.com'
        }
    })
        .then(response => response.blob())  // Đọc dữ liệu như file blob (hình ảnh)
        .then(imageBlob => {
            const imageObjectURL = URL.createObjectURL(imageBlob);
            document.getElementById('mainImage').src = imageObjectURL;
        })
        .catch(error => console.error('Error fetching image:', error));
}

// Đảm bảo rằng hàm này có thể được gọi từ các file cshtml khác
