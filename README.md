# 👟 Hệ thống E-commerce Bán Giày

Dự án xây dựng website bán giày trực tuyến với hệ thống quản lý biến thể phức tạp, giỏ hàng, thanh toán và khuyến mãi.

## 🚀 Stack Công nghệ
*   **Backend:** ASP.NET Core Web API (C#)
*   **Frontend:** ReactJS
*   **Kiến trúc Backend:** Package by Feature (Chia thư mục theo Module chức năng)

## 📈 Tiến độ Dự án

### Giai đoạn 1: Khởi tạo Nền tảng (Platform Setup)
- [x] Thiết kế Lược đồ CSDL (DBML) cho Sản phẩm, Biến thể, Đơn hàng, Khuyến mãi.
- [x] Thiết lập `design-token.md` và `global-rules.md` cho Frontend.
- [x] Khởi tạo cấu trúc thư mục Entity tập trung (`Infrastructure\Database\Entities`).
- [x] Thiết lập `BaseEntity` và các Entity cốt lõi.
- [x] Cấu hình `AppDbContext` và kết nối SQL Server.
- [x] Chạy Migration khởi tạo.
- [x] Setup Swagger/OpenAPI để test API.

### Giai đoạn 2: Phát triển Phân hệ Sản phẩm (Catalog & Variants)
- [x] Module Categories (Danh mục - CRUD API).
- [x] Cấu hình AutoMapper.
- [x] Module Brands (Thương hiệu - CRUD API).
- [x] Module Products & Product Variants (Sản phẩm và Biến thể Size/Màu).
- [x] Tích hợp Upload hình ảnh (Local/Cloudinary).

### Giai đoạn 3: Phân hệ Người dùng & Mua sắm (User & Shopping)
- [x] Đăng ký / Đăng nhập (JWT Authentication).
- [x] Giỏ hàng (Cart & Cart Items).
- [x] Quản lý Sổ địa chỉ (Address).

### Giai đoạn 4: Đơn hàng & Khuyến mãi (Orders & Promotions)
- [ ] Áp dụng mã giảm giá (Vouchers).
- [ ] Xử lý Checkout & Transaction trừ tồn kho.
- [ ] Tích hợp thanh toán.


# 🎨 Kế hoạch Triển khai Frontend (ReactJS) - ShoesShop

## 🚀 Công nghệ sử dụng
* **Core:** ReactJS (Build bằng Vite cho tốc độ siêu nhanh).
* **Styling:** Tailwind CSS (Tích hợp hoàn hảo với file `design-token.md` đã viết).
* **Routing:** React Router v6.
* **State Management:** Zustand (Hoặc Redux Toolkit) để quản lý Giỏ hàng và Auth.
* **HTTP Client:** Axios (Tạo Interceptor để tự động đính kèm JWT Token).

---

## 📈 Lộ trình Phát triển (Checklist)

### Giai đoạn 1: Khởi tạo & Cấu hình Nền tảng (Platform Setup)
- [ ] Khởi tạo dự án bằng Vite (`npm create vite@latest`).
- [ ] Cài đặt Tailwind CSS và cấu hình `tailwind.config.js` dựa trên `design-token.md` (Colors, Typography, Shadows).
- [ ] Cấu hình Axios Instance (Base URL: `http://localhost:5080/api`).
- [ ] Thiết lập Router cơ bản (Public Layout & Admin Layout).
- [ ] Dựng các UI Components dùng chung (Button, Input, Modal, Toast Notification).

### Giai đoạn 2: Phân hệ Khách hàng - Trải nghiệm mua sắm (Storefront)
- [ ] **Trang Chủ (Home):** Banner động, Hiển thị danh mục nổi bật, Slider Giày bán chạy.
- [ ] **Trang Danh mục (Shop):**
  - [ ] Gọi API `GET /api/products`.
  - [ ] Hiển thị lưới sản phẩm (Product Card).
  - [ ] Xây dựng bộ lọc (Filter) theo Category, Brand.
- [ ] **Trang Chi tiết Sản phẩm (Product Detail):**
  - [ ] Gọi API `GET /api/products/{id}`.
  - [ ] Logic UI quan trọng: Chuyển đổi màu sắc/size sẽ hiển thị đúng số lượng Tồn kho và Giá (dựa vào mảng `variants`).
  - [ ] Xử lý ảnh Carousel (Ảnh to + Thumbnail).

### Giai đoạn 3: Phân hệ Người dùng (Auth & Profile)
- [ ] Xây dựng form Đăng ký / Đăng nhập.
- [ ] Gọi API Auth, lưu JWT Token vào `localStorage` hoặc `cookie`.
- [ ] Cấu hình Axios Interceptor để mọi request sau này tự động có `Bearer {token}`.
- [ ] **Trang Tài khoản (My Account):**
  - [ ] Quản lý Sổ địa chỉ (Gọi API Address).
  - [ ] Xem Lịch sử đơn hàng (Gọi API Order History).

### Giai đoạn 4: Giỏ hàng & Thanh toán (Cart & Checkout)
- [ ] **Giỏ hàng (Cart Drawer / Cart Page):**
  - [ ] Gọi API `POST /api/cart` khi bấm "Thêm vào giỏ".
  - [ ] Cập nhật số lượng, xóa sản phẩm khỏi giỏ.
- [ ] **Trang Thanh toán (Checkout):**
  - [ ] Giao diện chọn Địa chỉ giao hàng.
  - [ ] Ô nhập Mã giảm giá -> Gọi API `POST /api/promotions/apply` -> Tính lại tổng tiền.
  - [ ] Chọn phương thức thanh toán (COD / VNPay).
  - [ ] Gọi API `POST /api/orders/checkout`.
- [ ] Xử lý trang `VnPayReturn` (Hiển thị thông báo Thành công / Thất bại khi từ VNPay quay về).

### Giai đoạn 5: Phân hệ Quản trị (Admin Portal) - (Tách biệt Route)
- [ ] Cấu hình Private Route (Chỉ user có `role = admin` mới được vào).
- [ ] Quản lý Sản phẩm:
  - [ ] Bảng danh sách sản phẩm.
  - [ ] Form tạo Sản phẩm (Có chức năng Upload ảnh gọi API `/api/uploads/image`).
  - [ ] Form thêm các Biến thể (Size/Màu) linh hoạt.
- [ ] Quản lý Đơn hàng: Xem danh sách và Đổi trạng thái (Pending -> Shipping).