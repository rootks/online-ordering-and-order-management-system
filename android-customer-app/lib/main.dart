import 'package:flutter/material.dart';
import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_auth/firebase_auth.dart';
import 'package:cloud_firestore/cloud_firestore.dart';
import 'firebase_options.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  try {
    await Firebase.initializeApp(
      options: DefaultFirebaseOptions.currentPlatform,
    );
  } catch (e) {
    print("Error initializing Firebase: $e");
  }

  runApp(const MyApp());
}

// ==========================================================
// MAIN APP
// ==========================================================

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      title: "Food Delivery",
      theme: ThemeData(
        primarySwatch: Colors.orange,
        useMaterial3: true,
      ),
      home: const AuthChecker(),
    );
  }
}

// ==========================================================
// AUTH CHECKER
// ==========================================================

class AuthChecker extends StatelessWidget {
  const AuthChecker({super.key});

  @override
  Widget build(BuildContext context) {
    return StreamBuilder<User?>(
      stream: FirebaseAuth.instance.authStateChanges(),
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const Scaffold(
            body: Center(
              child: CircularProgressIndicator(),
            ),
          );
        }

        if (!snapshot.hasData) {
          return const LoginPage();
        }

        return const RoleChecker();
      },
    );
  }
}

// ==========================================================
// ROLE CHECKER
// ==========================================================

class RoleChecker extends StatelessWidget {
  const RoleChecker({super.key});

  @override
  Widget build(BuildContext context) {
    User? user = FirebaseAuth.instance.currentUser;
    
    if (user == null) {
      return const LoginPage();
    }

    return FutureBuilder<DocumentSnapshot>(
      future: FirebaseFirestore.instance
          .collection("users")
          .doc(user.uid)
          .get(),
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const Scaffold(
            body: Center(
              child: CircularProgressIndicator(),
            ),
          );
        }

        if (!snapshot.hasData || !snapshot.data!.exists) {
          return const LoginPage();
        }

        Map<String, dynamic> data = snapshot.data!.data() as Map<String, dynamic>;
        String role = data["role"] ?? "customer";

        if (role == "rider") {
          return const RiderHomePage();
        }

        return const CustomerHomePage();
      },
    );
  }
}

// ==========================================================
// LOGIN PAGE
// ==========================================================

class LoginPage extends StatefulWidget {
  const LoginPage({super.key});

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final phoneController = TextEditingController();
  final passwordController = TextEditingController();
  bool loading = false;

  Future<void> login() async {
    if (phoneController.text.isEmpty || passwordController.text.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text("Enter all fields"),
          backgroundColor: Colors.red,
        ),
      );
      return;
    }

    setState(() {
      loading = true;
    });

    try {
      String email = "${phoneController.text.trim()}@delivery.app";
      await FirebaseAuth.instance.signInWithEmailAndPassword(
        email: email,
        password: passwordController.text.trim(),
      );
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(e.toString()),
          backgroundColor: Colors.red,
        ),
      );
      setState(() {
        loading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Center(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(25),
          child: Column(
            children: [
              const Icon(
                Icons.delivery_dining,
                size: 120,
                color: Colors.orange,
              ),
              const SizedBox(height: 20),
              const Text(
                "Food Delivery",
                style: TextStyle(
                  fontSize: 30,
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 40),
              TextField(
                controller: phoneController,
                keyboardType: TextInputType.phone,
                decoration: const InputDecoration(
                  labelText: "Mobile Number",
                  border: OutlineInputBorder(),
                  prefixIcon: Icon(Icons.phone),
                ),
              ),
              const SizedBox(height: 20),
              TextField(
                controller: passwordController,
                obscureText: true,
                decoration: const InputDecoration(
                  labelText: "Password",
                  border: OutlineInputBorder(),
                  prefixIcon: Icon(Icons.lock),
                ),
              ),
              const SizedBox(height: 30),
              loading
                  ? const CircularProgressIndicator()
                  : SizedBox(
                      width: double.infinity,
                      child: ElevatedButton(
                        onPressed: login,
                        style: ElevatedButton.styleFrom(
                          padding: const EdgeInsets.all(15),
                        ),
                        child: const Text(
                          "LOGIN",
                          style: TextStyle(fontSize: 16),
                        ),
                      ),
                    ),
              const SizedBox(height: 15),
              TextButton(
                onPressed: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (_) => const RegisterPage(),
                    ),
                  );
                },
                child: const Text(
                  "Create Account",
                  style: TextStyle(fontSize: 16),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

// ==========================================================
// REGISTER PAGE
// ==========================================================

class RegisterPage extends StatefulWidget {
  const RegisterPage({super.key});

  @override
  State<RegisterPage> createState() => _RegisterPageState();
}

class _RegisterPageState extends State<RegisterPage> {
  final nameController = TextEditingController();
  final phoneController = TextEditingController();
  final passwordController = TextEditingController();
  String role = "customer";
  bool loading = false;

  Future<void> register() async {
    if (nameController.text.isEmpty ||
        phoneController.text.isEmpty ||
        passwordController.text.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text("Please fill all fields"),
          backgroundColor: Colors.red,
        ),
      );
      return;
    }

    setState(() {
      loading = true;
    });

    try {
      String email = "${phoneController.text.trim()}@delivery.app";
      UserCredential userCredential = await FirebaseAuth.instance
          .createUserWithEmailAndPassword(
        email: email,
        password: passwordController.text.trim(),
      );

      await FirebaseFirestore.instance
          .collection("users")
          .doc(userCredential.user!.uid)
          .set({
        "name": nameController.text.trim(),
        "mobile": phoneController.text.trim(),
        "role": role,
        "createdAt": FieldValue.serverTimestamp(),
      });

      if (!mounted) return;
      
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text("Account created successfully!"),
          backgroundColor: Colors.green,
        ),
      );
      
      Navigator.pop(context);
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text("Error: ${e.toString()}"),
          backgroundColor: Colors.red,
        ),
      );
    } finally {
      if (mounted) {
        setState(() {
          loading = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Register"),
        backgroundColor: Colors.orange,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(25),
        child: Column(
          children: [
            TextField(
              controller: nameController,
              decoration: const InputDecoration(
                labelText: "Name",
                border: OutlineInputBorder(),
                prefixIcon: Icon(Icons.person),
              ),
            ),
            const SizedBox(height: 20),
            TextField(
              controller: phoneController,
              keyboardType: TextInputType.phone,
              decoration: const InputDecoration(
                labelText: "Phone",
                border: OutlineInputBorder(),
                prefixIcon: Icon(Icons.phone),
              ),
            ),
            const SizedBox(height: 20),
            TextField(
              controller: passwordController,
              obscureText: true,
              decoration: const InputDecoration(
                labelText: "Password",
                border: OutlineInputBorder(),
                prefixIcon: Icon(Icons.lock),
              ),
            ),
            const SizedBox(height: 20),
            DropdownButtonFormField<String>(
              value: role,
              decoration: const InputDecoration(
                labelText: "Role",
                border: OutlineInputBorder(),
                prefixIcon: Icon(Icons.person_outline),
              ),
              items: const [
                DropdownMenuItem(
                  value: "customer",
                  child: Text("Customer"),
                ),
                DropdownMenuItem(
                  value: "rider",
                  child: Text("Rider"),
                ),
              ],
              onChanged: (value) {
                setState(() {
                  role = value!;
                });
              },
            ),
            const SizedBox(height: 30),
            loading
                ? const CircularProgressIndicator()
                : SizedBox(
                    width: double.infinity,
                    child: ElevatedButton(
                      onPressed: register,
                      style: ElevatedButton.styleFrom(
                        padding: const EdgeInsets.all(15),
                        backgroundColor: Colors.orange,
                      ),
                      child: const Text(
                        "REGISTER",
                        style: TextStyle(fontSize: 16),
                      ),
                    ),
                  ),
          ],
        ),
      ),
    );
  }
}

// ==========================================================
// CUSTOMER HOME
// ==========================================================

class CustomerHomePage extends StatefulWidget {
  const CustomerHomePage({super.key});

  @override
  State<CustomerHomePage> createState() => _CustomerHomePageState();
}

class _CustomerHomePageState extends State<CustomerHomePage> {
  String search = "";

  @override
  Widget build(BuildContext context) {
    User? user = FirebaseAuth.instance.currentUser;
    
    if (user == null) {
      return const LoginPage();
    }

    return Scaffold(
      appBar: AppBar(
        title: const Text("Foods"),
        backgroundColor: Colors.orange,
        actions: [
          IconButton(
            icon: const Icon(Icons.history),
            onPressed: () {
              Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (_) => OrderHistoryPage(
                    userId: user.uid,
                  ),
                ),
              );
            },
          ),
          IconButton(
            icon: const Icon(Icons.shopping_cart),
            onPressed: () {
              showModalBottomSheet(
                context: context,
                isScrollControlled: true,
                shape: const RoundedRectangleBorder(
                  borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
                ),
                builder: (_) => CartSheet(
                  userId: user.uid,
                ),
              );
            },
          ),
          IconButton(
            onPressed: () async {
              await FirebaseAuth.instance.signOut();
            },
            icon: const Icon(Icons.logout),
          ),
        ],
      ),
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.all(10),
            child: TextField(
              decoration: const InputDecoration(
                hintText: "Search food",
                prefixIcon: Icon(Icons.search),
                border: OutlineInputBorder(),
              ),
              onChanged: (value) {
                setState(() {
                  search = value.toLowerCase();
                });
              },
            ),
          ),
          Expanded(
            child: StreamBuilder<QuerySnapshot>(
              stream: FirebaseFirestore.instance
                  .collection("product")
                  .snapshots(),
              builder: (context, snapshot) {
                if (snapshot.connectionState == ConnectionState.waiting) {
                  return const Center(
                    child: CircularProgressIndicator(),
                  );
                }

                if (snapshot.hasError) {
                  return Center(
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        const Icon(Icons.error, size: 50, color: Colors.red),
                        const SizedBox(height: 10),
                        Text("Error: ${snapshot.error}"),
                        ElevatedButton(
                          onPressed: () {},
                          child: const Text("Retry"),
                        ),
                      ],
                    ),
                  );
                }

                if (!snapshot.hasData || snapshot.data!.docs.isEmpty) {
                  return const Center(
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        Icon(Icons.fastfood, size: 80, color: Colors.grey),
                        SizedBox(height: 10),
                        Text(
                          "No products available",
                          style: TextStyle(fontSize: 18, color: Colors.grey),
                        ),
                      ],
                    ),
                  );
                }

                var products = snapshot.data!.docs;
                products = products.where((e) {
                  var data = e.data() as Map<String, dynamic>;
                  String name = data["name"]?.toString().toLowerCase() ?? "";
                  return name.contains(search);
                }).toList();

                if (products.isEmpty) {
                  return const Center(
                    child: Text("No products found"),
                  );
                }

                return GridView.builder(
                  padding: const EdgeInsets.all(10),
                  itemCount: products.length,
                  gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                    crossAxisCount: 2,
                    childAspectRatio: 0.7,
                    crossAxisSpacing: 10,
                    mainAxisSpacing: 10,
                  ),
                  itemBuilder: (context, index) {
                    var data = products[index].data() as Map<String, dynamic>;
                    String productName = data["name"] ?? "Product";
                    double price = (data["price"] ?? 0).toDouble();
                    String imageUrl = data["image"] ?? "";
                    
                    return Card(
                      elevation: 3,
                      child: Padding(
                        padding: const EdgeInsets.all(10),
                        child: Column(
                          children: [
                            Expanded(
                              child: imageUrl.isNotEmpty
                                  ? Image.network(
                                      imageUrl,
                                      fit: BoxFit.cover,
                                      errorBuilder: (context, error, stackTrace) {
                                        return Container(
                                          color: Colors.grey[200],
                                          child: const Icon(
                                            Icons.fastfood,
                                            size: 80,
                                            color: Colors.grey,
                                          ),
                                        );
                                      },
                                    )
                                  : Container(
                                      color: Colors.grey[200],
                                      child: const Icon(
                                        Icons.fastfood,
                                        size: 80,
                                        color: Colors.grey,
                                      ),
                                    ),
                            ),
                            const SizedBox(height: 10),
                            Text(
                              productName,
                              style: const TextStyle(
                                fontWeight: FontWeight.bold,
                                fontSize: 14,
                              ),
                              textAlign: TextAlign.center,
                              maxLines: 2,
                              overflow: TextOverflow.ellipsis,
                            ),
                            Text(
                              "Rs. ${price.toStringAsFixed(2)}",
                              style: const TextStyle(
                                color: Colors.orange,
                                fontWeight: FontWeight.bold,
                                fontSize: 16,
                              ),
                            ),
                            const SizedBox(height: 10),
                            SizedBox(
                              width: double.infinity,
                              child: ElevatedButton(
                                style: ElevatedButton.styleFrom(
                                  backgroundColor: Colors.orange,
                                  padding: const EdgeInsets.symmetric(vertical: 8),
                                ),
                                onPressed: () async {
                                  try {
                                    // Check if product already in cart
                                    final existingCart = await FirebaseFirestore
                                        .instance
                                        .collection("cart")
                                        .where("userId", isEqualTo: user.uid)
                                        .where("productId", isEqualTo: products[index].id)
                                        .get();

                                    if (existingCart.docs.isNotEmpty) {
                                      // Update quantity
                                      await existingCart.docs.first.reference.update({
                                        "quantity": FieldValue.increment(1)
                                      });
                                      ScaffoldMessenger.of(context).showSnackBar(
                                        SnackBar(
                                          content: Text("$productName quantity increased"),
                                          duration: const Duration(seconds: 1),
                                          backgroundColor: Colors.green,
                                        ),
                                      );
                                    } else {
                                      // Add new item
                                      await FirebaseFirestore.instance
                                          .collection("cart")
                                          .add({
                                        "userId": user.uid,
                                        "productId": products[index].id,
                                        "productName": productName,
                                        "price": price,
                                        "image": imageUrl,
                                        "quantity": 1,
                                        "addedAt": FieldValue.serverTimestamp(),
                                      });
                                      ScaffoldMessenger.of(context).showSnackBar(
                                        SnackBar(
                                          content: Text("$productName added to cart"),
                                          duration: const Duration(seconds: 1),
                                          backgroundColor: Colors.green,
                                        ),
                                      );
                                    }
                                  } catch (e) {
                                    ScaffoldMessenger.of(context).showSnackBar(
                                      SnackBar(
                                        content: Text("Error: $e"),
                                        backgroundColor: Colors.red,
                                      ),
                                    );
                                  }
                                },
                                child: const Text(
                                  "Add to Cart",
                                  style: TextStyle(fontSize: 12),
                                ),
                              ),
                            ),
                          ],
                        ),
                      ),
                    );
                  },
                );
              },
            ),
          ),
        ],
      ),
    );
  }
}

// ==========================================================
// CART SHEET - FIXED AND STABLE
// ==========================================================

class CartSheet extends StatefulWidget {
  final String userId;

  const CartSheet({
    super.key,
    required this.userId,
  });

  @override
  State<CartSheet> createState() => _CartSheetState();
}

class _CartSheetState extends State<CartSheet> {
  final addressController = TextEditingController();
  bool isPlacingOrder = false;

  @override
  void dispose() {
    addressController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      height: MediaQuery.of(context).size.height * 0.9,
      decoration: const BoxDecoration(
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      child: Column(
        children: [
          Container(
            padding: const EdgeInsets.all(15),
            decoration: BoxDecoration(
              color: Colors.orange,
              borderRadius: const BorderRadius.vertical(top: Radius.circular(20)),
            ),
            child: const Center(
              child: Text(
                "My Cart",
                style: TextStyle(
                  fontSize: 24,
                  fontWeight: FontWeight.bold,
                  color: Colors.white,
                ),
              ),
            ),
          ),
          Expanded(
            child: StreamBuilder<QuerySnapshot>(
              stream: FirebaseFirestore.instance
                  .collection("cart")
                  .where("userId", isEqualTo: widget.userId)
                  .snapshots(),
              builder: (context, snapshot) {
                if (snapshot.connectionState == ConnectionState.waiting) {
                  return const Center(
                    child: CircularProgressIndicator(),
                  );
                }

                if (snapshot.hasError) {
                  return Center(
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        const Icon(Icons.error, size: 50, color: Colors.red),
                        const SizedBox(height: 10),
                        Text("Error: ${snapshot.error}"),
                        ElevatedButton(
                          onPressed: () {
                            setState(() {});
                          },
                          child: const Text("Retry"),
                        ),
                      ],
                    ),
                  );
                }

                if (!snapshot.hasData) {
                  return const Center(
                    child: Text("No items in cart"),
                  );
                }

                var cartItems = snapshot.data!.docs;
                
                if (cartItems.isEmpty) {
                  return const Center(
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        Icon(Icons.shopping_cart, size: 80, color: Colors.grey),
                        SizedBox(height: 10),
                        Text(
                          "Your cart is empty",
                          style: TextStyle(fontSize: 18, color: Colors.grey),
                        ),
                        SizedBox(height: 10),
                        Text(
                          "Add some items from the menu",
                          style: TextStyle(color: Colors.grey),
                        ),
                      ],
                    ),
                  );
                }

                double total = 0;
                for (var item in cartItems) {
                  try {
                    var data = item.data() as Map<String, dynamic>;
                    double price = (data["price"] ?? 0).toDouble();
                    int quantity = data["quantity"] ?? 1;
                    total += price * quantity;
                  } catch (e) {
                    print("Error calculating total: $e");
                  }
                }

                return Column(
                  children: [
                    Expanded(
                      child: ListView.builder(
                        padding: const EdgeInsets.all(10),
                        itemCount: cartItems.length,
                        itemBuilder: (context, index) {
                          try {
                            var data = cartItems[index].data() as Map<String, dynamic>;
                            String productName = data["productName"] ?? "Unknown Product";
                            double price = (data["price"] ?? 0).toDouble();
                            int quantity = data["quantity"] ?? 1;
                            String imageUrl = data["image"] ?? "";
                            
                            return Card(
                              margin: const EdgeInsets.symmetric(vertical: 5),
                              child: ListTile(
                                leading: ClipRRect(
                                  borderRadius: BorderRadius.circular(8),
                                  child: imageUrl.isNotEmpty
                                      ? Image.network(
                                          imageUrl,
                                          width: 60,
                                          height: 60,
                                          fit: BoxFit.cover,
                                          errorBuilder: (context, error, stackTrace) {
                                            return Container(
                                              width: 60,
                                              height: 60,
                                              color: Colors.grey[300],
                                              child: const Icon(Icons.fastfood),
                                            );
                                          },
                                        )
                                      : Container(
                                          width: 60,
                                          height: 60,
                                          color: Colors.grey[300],
                                          child: const Icon(Icons.fastfood),
                                        ),
                                ),
                                title: Text(
                                  productName,
                                  style: const TextStyle(fontWeight: FontWeight.bold),
                                ),
                                subtitle: Text("Rs. ${price.toStringAsFixed(2)}"),
                                trailing: Row(
                                  mainAxisSize: MainAxisSize.min,
                                  children: [
                                    IconButton(
                                      icon: const Icon(Icons.remove, size: 20),
                                      onPressed: () async {
                                        try {
                                          if (quantity > 1) {
                                            await cartItems[index].reference.update({
                                              "quantity": FieldValue.increment(-1)
                                            });
                                          } else {
                                            await cartItems[index].reference.delete();
                                          }
                                        } catch (e) {
                                          print("Error updating quantity: $e");
                                        }
                                      },
                                    ),
                                    Container(
                                      padding: const EdgeInsets.symmetric(horizontal: 8),
                                      child: Text(
                                        quantity.toString(),
                                        style: const TextStyle(
                                          fontSize: 16,
                                          fontWeight: FontWeight.bold,
                                        ),
                                      ),
                                    ),
                                    IconButton(
                                      icon: const Icon(Icons.add, size: 20),
                                      onPressed: () async {
                                        try {
                                          await cartItems[index].reference.update({
                                            "quantity": FieldValue.increment(1)
                                          });
                                        } catch (e) {
                                          print("Error updating quantity: $e");
                                        }
                                      },
                                    ),
                                  ],
                                ),
                              ),
                            );
                          } catch (e) {
                            return Card(
                              child: ListTile(
                                title: const Text("Error loading item"),
                                subtitle: Text(e.toString()),
                                trailing: IconButton(
                                  icon: const Icon(Icons.delete, color: Colors.red),
                                  onPressed: () async {
                                    await cartItems[index].reference.delete();
                                  },
                                ),
                              ),
                            );
                          }
                        },
                      ),
                    ),
                    Container(
                      padding: const EdgeInsets.all(15),
                      decoration: BoxDecoration(
                        color: Colors.white,
                        boxShadow: [
                          BoxShadow(
                            color: Colors.grey.withOpacity(0.3),
                            blurRadius: 10,
                            offset: const Offset(0, -5),
                          ),
                        ],
                      ),
                      child: Column(
                        children: [
                          TextField(
                            controller: addressController,
                            decoration: const InputDecoration(
                              labelText: "Delivery Address",
                              hintText: "Enter your complete address",
                              border: OutlineInputBorder(),
                              prefixIcon: Icon(Icons.location_on),
                            ),
                            maxLines: 2,
                          ),
                          const SizedBox(height: 15),
                          Container(
                            padding: const EdgeInsets.all(15),
                            decoration: BoxDecoration(
                              color: Colors.orange.shade50,
                              borderRadius: BorderRadius.circular(10),
                            ),
                            child: Row(
                              mainAxisAlignment: MainAxisAlignment.spaceBetween,
                              children: [
                                const Text(
                                  "Total Amount:",
                                  style: TextStyle(
                                    fontSize: 18,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                                Text(
                                  "Rs. ${total.toStringAsFixed(2)}",
                                  style: const TextStyle(
                                    fontSize: 20,
                                    fontWeight: FontWeight.bold,
                                    color: Colors.orange,
                                  ),
                                ),
                              ],
                            ),
                          ),
                          const SizedBox(height: 15),
                          SizedBox(
                            width: double.infinity,
                            child: ElevatedButton(
                              style: ElevatedButton.styleFrom(
                                backgroundColor: Colors.orange,
                                padding: const EdgeInsets.symmetric(vertical: 15),
                              ),
                              onPressed: isPlacingOrder
                                  ? null
                                  : () async {
                                      if (addressController.text.isEmpty) {
                                        ScaffoldMessenger.of(context).showSnackBar(
                                          const SnackBar(
                                            content: Text("Please enter delivery address"),
                                            backgroundColor: Colors.red,
                                          ),
                                        );
                                        return;
                                      }

                                      setState(() {
                                        isPlacingOrder = true;
                                      });

                                      try {
                                        List<Map<String, dynamic>> items = [];
                                        for (var item in cartItems) {
                                          var data = item.data() as Map<String, dynamic>;
                                          items.add({
                                            "productId": data["productId"] ?? item.id,
                                            "productName": data["productName"] ?? "Unknown",
                                            "price": (data["price"] ?? 0).toDouble(),
                                            "quantity": data["quantity"] ?? 1,
                                            "image": data["image"] ?? "",
                                          });
                                        }

                                        await FirebaseFirestore.instance
                                            .collection("orders")
                                            .add({
                                          "customerId": widget.userId,
                                          "items": items,
                                          "total": total,
                                          "address": addressController.text.trim(),
                                          "status": "Pending",
                                          "riderId": "",
                                          "createdAt": FieldValue.serverTimestamp(),
                                          "updatedAt": FieldValue.serverTimestamp(),
                                        });

                                        // Clear cart
                                        for (var item in cartItems) {
                                          await FirebaseFirestore.instance
                                              .collection("cart")
                                              .doc(item.id)
                                              .delete();
                                        }

                                        if (!mounted) return;
                                        
                                        ScaffoldMessenger.of(context).showSnackBar(
                                          const SnackBar(
                                            content: Text("Order Placed Successfully!"),
                                            backgroundColor: Colors.green,
                                            duration: Duration(seconds: 2),
                                          ),
                                        );
                                        
                                        Navigator.pop(context);
                                      } catch (e) {
                                        if (!mounted) return;
                                        ScaffoldMessenger.of(context).showSnackBar(
                                          SnackBar(
                                            content: Text("Error placing order: $e"),
                                            backgroundColor: Colors.red,
                                          ),
                                        );
                                      } finally {
                                        if (mounted) {
                                          setState(() {
                                            isPlacingOrder = false;
                                          });
                                        }
                                      }
                                    },
                              child: isPlacingOrder
                                  ? const CircularProgressIndicator(color: Colors.white)
                                  : const Text(
                                      "PLACE ORDER",
                                      style: TextStyle(
                                        fontSize: 16,
                                        fontWeight: FontWeight.bold,
                                      ),
                                    ),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                );
              },
            ),
          ),
        ],
      ),
    );
  }
}

// ==========================================================
// ORDER HISTORY
// ==========================================================

class OrderHistoryPage extends StatelessWidget {
  final String userId;

  const OrderHistoryPage({
    super.key,
    required this.userId,
  });

  Color getStatusColor(String status) {
    switch (status) {
      case "Pending":
        return Colors.orange;
      case "Preparing":
        return Colors.blue;
      case "Delivering":
        return Colors.purple;
      case "Delivered":
        return Colors.green;
      default:
        return Colors.grey;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("My Orders"),
        backgroundColor: Colors.orange,
      ),
      body: StreamBuilder<QuerySnapshot>(
        stream: FirebaseFirestore.instance
            .collection("orders")
            .where("customerId", isEqualTo: userId)
            .orderBy("createdAt", descending: true)
            .snapshots(),
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(
              child: CircularProgressIndicator(),
            );
          }

          if (snapshot.hasError) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const Icon(Icons.error, size: 50, color: Colors.red),
                  const SizedBox(height: 10),
                  Text("Error: ${snapshot.error}"),
                ],
              ),
            );
          }

          if (!snapshot.hasData || snapshot.data!.docs.isEmpty) {
            return const Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(Icons.history, size: 80, color: Colors.grey),
                  SizedBox(height: 20),
                  Text(
                    "No orders yet",
                    style: TextStyle(fontSize: 18, color: Colors.grey),
                  ),
                  SizedBox(height: 10),
                  Text(
                    "Start shopping to see your orders",
                    style: TextStyle(color: Colors.grey),
                  ),
                ],
              ),
            );
          }

          var orders = snapshot.data!.docs;

          return ListView.builder(
            itemCount: orders.length,
            itemBuilder: (context, index) {
              var order = orders[index].data() as Map<String, dynamic>;
              String status = order["status"] ?? "Pending";
              double total = (order["total"] ?? 0).toDouble();
              
              return Card(
                margin: const EdgeInsets.all(10),
                child: ExpansionTile(
                  leading: CircleAvatar(
                    backgroundColor: getStatusColor(status),
                    child: Text(
                      status.isNotEmpty ? status[0] : "?",
                      style: const TextStyle(color: Colors.white),
                    ),
                  ),
                  title: Text(
                    "Order #${orders[index].id.substring(0, 8)}",
                    style: const TextStyle(fontWeight: FontWeight.bold),
                  ),
                  subtitle: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text("Total: Rs. ${total.toStringAsFixed(2)}"),
                      const SizedBox(height: 5),
                      Container(
                        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                        decoration: BoxDecoration(
                          color: getStatusColor(status).withOpacity(0.1),
                          borderRadius: BorderRadius.circular(10),
                        ),
                        child: Text(
                          status,
                          style: TextStyle(
                            color: getStatusColor(status),
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ),
                    ],
                  ),
                  trailing: ElevatedButton(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.orange,
                    ),
                    child: const Text("Track"),
                    onPressed: () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (_) => OrderTrackingPage(
                            orderId: orders[index].id,
                          ),
                        ),
                      );
                    },
                  ),
                  children: [
                    Padding(
                      padding: const EdgeInsets.all(16),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const Text(
                            "Items:",
                            style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
                          ),
                          const SizedBox(height: 8),
                          ...List.generate(
                            (order["items"] as List?)?.length ?? 0,
                            (i) {
                              var item = (order["items"] as List)[i];
                              String productName = item["productName"] ?? "Unknown";
                              int quantity = item["quantity"] ?? 1;
                              double price = (item["price"] ?? 0).toDouble();
                              
                              return Padding(
                                padding: const EdgeInsets.symmetric(vertical: 4),
                                child: Row(
                                  children: [
                                    Expanded(
                                      child: Text(productName),
                                    ),
                                    Text("x$quantity"),
                                    const SizedBox(width: 10),
                                    Text(
                                      "Rs. ${(price * quantity).toStringAsFixed(2)}",
                                      style: const TextStyle(fontWeight: FontWeight.bold),
                                    ),
                                  ],
                                ),
                              );
                            },
                          ),
                          const Divider(),
                          Row(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              const Text(
                                "Delivery Address:",
                                style: TextStyle(fontWeight: FontWeight.bold),
                              ),
                              const SizedBox(width: 10),
                              Expanded(
                                child: Text(
                                  order["address"] ?? "No address provided",
                                  textAlign: TextAlign.right,
                                ),
                              ),
                            ],
                          ),
                          if (order["riderId"] != null && order["riderId"].toString().isNotEmpty)
                            Padding(
                              padding: const EdgeInsets.only(top: 8),
                              child: Row(
                                children: [
                                  const Icon(Icons.motorcycle, size: 16, color: Colors.grey),
                                  const SizedBox(width: 5),
                                  const Text("Rider assigned"),
                                  const SizedBox(width: 10),
                                  Expanded(
                                    child: Text(
                                      "ID: ${order["riderId"]}",
                                      style: const TextStyle(fontSize: 12, color: Colors.grey),
                                      overflow: TextOverflow.ellipsis,
                                    ),
                                  ),
                                ],
                              ),
                            ),
                        ],
                      ),
                    ),
                  ],
                ),
              );
            },
          );
        },
      ),
    );
  }
}

// ==========================================================
// ORDER TRACKING
// ==========================================================

class OrderTrackingPage extends StatelessWidget {
  final String orderId;

  const OrderTrackingPage({
    super.key,
    required this.orderId,
  });

  int getStep(String status) {
    switch (status) {
      case "Pending":
        return 0;
      case "Preparing":
        return 1;
      case "Delivering":
        return 2;
      case "Delivered":
        return 3;
      default:
        return 0;
    }
  }

  IconData getStatusIcon(String status) {
    switch (status) {
      case "Pending":
        return Icons.pending_actions;
      case "Preparing":
        return Icons.kitchen;
      case "Delivering":
        return Icons.delivery_dining;
      case "Delivered":
        return Icons.check_circle;
      default:
        return Icons.pending_actions;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Track Order"),
        backgroundColor: Colors.orange,
      ),
      body: StreamBuilder<DocumentSnapshot>(
        stream: FirebaseFirestore.instance
            .collection("orders")
            .doc(orderId)
            .snapshots(),
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(
              child: CircularProgressIndicator(),
            );
          }

          if (snapshot.hasError) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const Icon(Icons.error, size: 50, color: Colors.red),
                  const SizedBox(height: 10),
                  Text("Error: ${snapshot.error}"),
                  ElevatedButton(
                    onPressed: () {
                      Navigator.pop(context);
                    },
                    child: const Text("Go Back"),
                  ),
                ],
              ),
            );
          }

          if (!snapshot.hasData || !snapshot.data!.exists) {
            return const Center(
              child: Text("Order not found"),
            );
          }

          var order = snapshot.data!.data() as Map<String, dynamic>;
          String status = order["status"] ?? "Pending";

          return Column(
            children: [
              Expanded(
                child: Stepper(
                  currentStep: getStep(status),
                  controlsBuilder: (_, __) => const SizedBox(),
                  physics: const NeverScrollableScrollPhysics(),
                  steps: [
                    const Step(
                      title: Text("Pending"),
                      content: Text("Your order has been placed and waiting for confirmation"),
                      isActive: true,
                    ),
                    const Step(
                      title: Text("Preparing"),
                      content: Text("Restaurant is preparing your food"),
                      isActive: true,
                    ),
                    const Step(
                      title: Text("Delivering"),
                      content: Text("Rider is on the way to deliver your order"),
                      isActive: true,
                    ),
                    const Step(
                      title: Text("Delivered"),
                      content: Text("Your order has been delivered successfully"),
                      isActive: true,
                    ),
                  ],
                ),
              ),
              Container(
                margin: const EdgeInsets.all(20),
                padding: const EdgeInsets.all(20),
                decoration: BoxDecoration(
                  gradient: LinearGradient(
                    colors: [Colors.orange.shade300, Colors.orange.shade700],
                  ),
                  borderRadius: BorderRadius.circular(15),
                ),
                child: Row(
                  children: [
                    Icon(
                      getStatusIcon(status),
                      color: Colors.white,
                      size: 40,
                    ),
                    const SizedBox(width: 15),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            "Current Status: $status",
                            style: const TextStyle(
                              color: Colors.white,
                              fontWeight: FontWeight.bold,
                              fontSize: 18,
                            ),
                          ),
                          const SizedBox(height: 5),
                          if (status == "Pending")
                            const Text(
                              "Waiting for rider to accept your order",
                              style: TextStyle(color: Colors.white),
                            ),
                          if (status == "Preparing")
                            const Text(
                              "Your food is being prepared",
                              style: TextStyle(color: Colors.white),
                            ),
                          if (status == "Delivering")
                            const Text(
                              "Your rider is on the way!",
                              style: TextStyle(color: Colors.white),
                            ),
                          if (status == "Delivered")
                            const Text(
                              "Thank you for ordering with us!",
                              style: TextStyle(color: Colors.white),
                            ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            ],
          );
        },
      ),
    );
  }
}

// ==========================================================
// RIDER HOME
// ==========================================================

class RiderHomePage extends StatefulWidget {
  const RiderHomePage({super.key});

  @override
  State<RiderHomePage> createState() => _RiderHomePageState();
}

class _RiderHomePageState extends State<RiderHomePage> with SingleTickerProviderStateMixin {
  late TabController _tabController;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  Color getStatusColor(String status) {
    switch (status) {
      case "Pending":
        return Colors.orange;
      case "Preparing":
        return Colors.blue;
      case "Delivering":
        return Colors.purple;
      case "Delivered":
        return Colors.green;
      default:
        return Colors.grey;
    }
  }

  @override
  Widget build(BuildContext context) {
    User? rider = FirebaseAuth.instance.currentUser;
    
    if (rider == null) {
      return const LoginPage();
    }

    return Scaffold(
      appBar: AppBar(
        title: const Text("Rider Dashboard"),
        backgroundColor: Colors.orange,
        bottom: TabBar(
          controller: _tabController,
          tabs: const [
            Tab(text: "Active Orders"),
            Tab(text: "Completed"),
          ],
        ),
        actions: [
          IconButton(
            onPressed: () async {
              await FirebaseAuth.instance.signOut();
            },
            icon: const Icon(Icons.logout),
          ),
        ],
      ),
      body: TabBarView(
        controller: _tabController,
        children: [
          // Active Orders Tab
          StreamBuilder<QuerySnapshot>(
            stream: FirebaseFirestore.instance
                .collection("orders")
                .where("status", whereIn: ["Pending", "Preparing", "Delivering"])
                .orderBy("createdAt", descending: true)
                .snapshots(),
            builder: (context, snapshot) {
              if (snapshot.connectionState == ConnectionState.waiting) {
                return const Center(child: CircularProgressIndicator());
              }

              if (snapshot.hasError) {
                return Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      const Icon(Icons.error, size: 50, color: Colors.red),
                      const SizedBox(height: 10),
                      Text("Error: ${snapshot.error}"),
                    ],
                  ),
                );
              }

              if (!snapshot.hasData || snapshot.data!.docs.isEmpty) {
                return const Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Icon(Icons.inbox, size: 80, color: Colors.grey),
                      SizedBox(height: 10),
                      Text(
                        "No active orders",
                        style: TextStyle(fontSize: 18, color: Colors.grey),
                      ),
                    ],
                  ),
                );
              }

              var orders = snapshot.data!.docs;
              return ListView.builder(
                itemCount: orders.length,
                itemBuilder: (context, index) {
                  var order = orders[index].data() as Map<String, dynamic>;
                  return _buildOrderCard(
                    context,
                    order,
                    orders[index].id,
                    rider.uid,
                  );
                },
              );
            },
          ),
          
          // Completed Orders Tab
          StreamBuilder<QuerySnapshot>(
            stream: FirebaseFirestore.instance
                .collection("orders")
                .where("status", isEqualTo: "Delivered")
                .where("riderId", isEqualTo: rider.uid)
                .orderBy("createdAt", descending: true)
                .snapshots(),
            builder: (context, snapshot) {
              if (snapshot.connectionState == ConnectionState.waiting) {
                return const Center(child: CircularProgressIndicator());
              }

              if (snapshot.hasError) {
                return Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      const Icon(Icons.error, size: 50, color: Colors.red),
                      const SizedBox(height: 10),
                      Text("Error: ${snapshot.error}"),
                    ],
                  ),
                );
              }

              if (!snapshot.hasData || snapshot.data!.docs.isEmpty) {
                return const Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Icon(Icons.check_circle, size: 80, color: Colors.grey),
                      SizedBox(height: 10),
                      Text(
                        "No completed orders",
                        style: TextStyle(fontSize: 18, color: Colors.grey),
                      ),
                    ],
                  ),
                );
              }

              var orders = snapshot.data!.docs;
              return ListView.builder(
                itemCount: orders.length,
                itemBuilder: (context, index) {
                  var order = orders[index].data() as Map<String, dynamic>;
                  return _buildOrderCard(
                    context,
                    order,
                    orders[index].id,
                    rider.uid,
                    showActions: false,
                  );
                },
              );
            },
          ),
        ],
      ),
    );
  }

  Widget _buildOrderCard(
    BuildContext context,
    Map<String, dynamic> order,
    String orderId,
    String riderId, {
    bool showActions = true,
  }) {
    String status = order["status"] ?? "Pending";
    double total = (order["total"] ?? 0).toDouble();
    String address = order["address"] ?? "No address";
    List items = (order["items"] as List?) ?? [];
    String orderRiderId = order["riderId"] ?? "";
    
    return Card(
      margin: const EdgeInsets.all(10),
      child: Padding(
        padding: const EdgeInsets.all(15),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  "Order #${orderId.substring(0, 8)}",
                  style: const TextStyle(
                    fontWeight: FontWeight.bold,
                    fontSize: 16,
                  ),
                ),
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                  decoration: BoxDecoration(
                    color: getStatusColor(status).withOpacity(0.1),
                    borderRadius: BorderRadius.circular(10),
                  ),
                  child: Text(
                    status,
                    style: TextStyle(
                      color: getStatusColor(status),
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 10),
            Text(
              "Total: Rs. ${total.toStringAsFixed(2)}",
              style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 5),
            Row(
              children: [
                const Icon(Icons.location_on, size: 16, color: Colors.grey),
                const SizedBox(width: 5),
                Expanded(
                  child: Text(
                    address,
                    style: const TextStyle(fontSize: 12),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 10),
            const Text(
              "Items:",
              style: TextStyle(fontWeight: FontWeight.bold),
            ),
            ...List.generate(
              items.length,
              (i) {
                var item = items[i];
                String productName = item["productName"] ?? "Unknown";
                int quantity = item["quantity"] ?? 1;
                return Padding(
                  padding: const EdgeInsets.only(left: 10, top: 5),
                  child: Text("• $productName x$quantity"),
                );
              },
            ),
            if (showActions) ...[
              const SizedBox(height: 15),
              if (status == "Pending")
                SizedBox(
                  width: double.infinity,
                  child: ElevatedButton(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.green,
                    ),
                    onPressed: () async {
                      try {
                        await FirebaseFirestore.instance
                            .collection("orders")
                            .doc(orderId)
                            .update({
                          "status": "Preparing",
                          "riderId": riderId,
                          "updatedAt": FieldValue.serverTimestamp(),
                        });
                        
                        ScaffoldMessenger.of(context).showSnackBar(
                          const SnackBar(
                            content: Text("Order accepted. Status changed to Preparing"),
                            backgroundColor: Colors.green,
                          ),
                        );
                      } catch (e) {
                        ScaffoldMessenger.of(context).showSnackBar(
                          SnackBar(
                            content: Text("Error: $e"),
                            backgroundColor: Colors.red,
                          ),
                        );
                      }
                    },
                    child: const Text("ACCEPT ORDER"),
                  ),
                ),
              
              if (status == "Preparing" && orderRiderId == riderId)
                SizedBox(
                  width: double.infinity,
                  child: ElevatedButton(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.orange,
                    ),
                    onPressed: () async {
                      try {
                        await FirebaseFirestore.instance
                            .collection("orders")
                            .doc(orderId)
                            .update({
                          "status": "Delivering",
                          "updatedAt": FieldValue.serverTimestamp(),
                        });
                        
                        ScaffoldMessenger.of(context).showSnackBar(
                          const SnackBar(
                            content: Text("Order is out for delivery"),
                            backgroundColor: Colors.orange,
                          ),
                        );
                      } catch (e) {
                        ScaffoldMessenger.of(context).showSnackBar(
                          SnackBar(
                            content: Text("Error: $e"),
                            backgroundColor: Colors.red,
                          ),
                        );
                      }
                    },
                    child: const Text("START DELIVERY"),
                  ),
                ),
              
              if (status == "Delivering" && orderRiderId == riderId)
                SizedBox(
                  width: double.infinity,
                  child: ElevatedButton(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.blue,
                    ),
                    onPressed: () async {
                      try {
                        await FirebaseFirestore.instance
                            .collection("orders")
                            .doc(orderId)
                            .update({
                          "status": "Delivered",
                          "updatedAt": FieldValue.serverTimestamp(),
                        });
                        
                        ScaffoldMessenger.of(context).showSnackBar(
                          const SnackBar(
                            content: Text("Order delivered successfully!"),
                            backgroundColor: Colors.green,
                          ),
                        );
                      } catch (e) {
                        ScaffoldMessenger.of(context).showSnackBar(
                          SnackBar(
                            content: Text("Error: $e"),
                            backgroundColor: Colors.red,
                          ),
                        );
                      }
                    },
                    child: const Text("MARK AS DELIVERED"),
                  ),
                ),
            ],
          ],
        ),
      ),
    );
  }
}