# 👟 Hệ thống E-commerce Bán Giày

Dự án xây dựng website bán giày trực tuyến với hệ thống quản lý biến thể phức tạp, giỏ hàng, thanh toán và khuyến mãi.

## 🚀 Stack Công nghệ
*   **Backend:** ASP.NET Core Web API (C#)
*   **Frontend:** ReactJS
*   **Kiến trúc Backend:** Package by Feature (Chia thư mục theo Module chức năng)

## 📈 Tiến độ Dự án

### Giai đoạn 1: Khởi tạo Nền tảng (Platform Setup)
- [x] Thiết kế Lược đồ CSDL (DBML) cho Sản phẩm, Biến thể, Đơn hàng.
- [x] Thiết lập `design-token.md` và `global-rules.md` cho Frontend.
- [ ] Khởi tạo dự án ASP.NET Core Web API.
- [ ] Thiết lập `BaseEntity` và tự động hóa Tracking Time (CreatedAt, UpdatedAt).
- [ ] Cấu hình kết nối Database và Migration khởi tạo.
- [ ] Setup Swagger/OpenAPI để test API.

### Giai đoạn 2: Phát triển Phân hệ Sản phẩm (Catalog & Variants)
- [ ] Module Categories (Danh mục).
- [ ] Module Brands (Thương hiệu).
- [ ] Module Products & Product Variants (Sản phẩm và Biến thể Size/Màu).
- [ ] Tích hợp Upload hình ảnh (Local/Cloudinary).

### Giai đoạn 3: Phân hệ Người dùng & Mua sắm (User & Shopping)
- [ ] Đăng ký / Đăng nhập (JWT Authentication).
- [ ] Giỏ hàng (Cart & Cart Items).
- [ ] Quản lý Sổ địa chỉ (Address).

### Giai đoạn 4: Đơn hàng & Khuyến mãi (Orders & Promotions)
- [ ] Áp dụng mã giảm giá (Vouchers).
- [ ] Xử lý Checkout & Transaction trừ tồn kho.
- [ ] Tích hợp thanh toán.