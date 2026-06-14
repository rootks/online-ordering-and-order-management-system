=// ==========================================================
// COMPLETE FOOD DELIVERY APP
// FLUTTER + FIREBASE + CUSTOMER + RIDER
// UPDATED STATUS: Pending, Preparing, Delivering, Delivered
// ==========================================================

import 'package:flutter/material.dart';
import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_auth/firebase_auth.dart';
import 'package:cloud_firestore/cloud_firestore.dart';
import 'firebase_options.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  await Firebase.initializeApp(
    options: DefaultFirebaseOptions.currentPlatform,
  );

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
    User user = FirebaseAuth.instance.currentUser!;

    return FutureBuilder<DocumentSnapshot>(
      future: FirebaseFirestore.instance
          .collection("users")
          .doc(user.uid)
          .get(),
      builder: (context, snapshot) {
        if (!snapshot.hasData) {
          return const Scaffold(
            body: Center(
              child: CircularProgressIndicator(),
            ),
          );
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
        ),
      );
    }

    setState(() {
      loading = false;
    });
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
                decoration: const InputDecoration(
                  labelText: "Mobile Number",
                  border: OutlineInputBorder(),
                ),
              ),
              const SizedBox(height: 20),
              TextField(
                controller: passwordController,
                obscureText: true,
                decoration: const InputDecoration(
                  labelText: "Password",
                  border: OutlineInputBorder(),
                ),
              ),
              const SizedBox(height: 30),
              loading
                  ? const CircularProgressIndicator()
                  : SizedBox(
                      width: double.infinity,
                      child: ElevatedButton(
                        onPressed: login,
                        child: const Padding(
                          padding: EdgeInsets.all(15),
                          child: Text("LOGIN"),
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
                child: const Text("Create Account"),
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
      });

      if (!mounted) return;
      Navigator.pop(context);
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(e.toString()),
        ),
      );
    }

    setState(() {
      loading = false;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Register"),
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
              ),
            ),
            const SizedBox(height: 20),
            TextField(
              controller: phoneController,
              decoration: const InputDecoration(
                labelText: "Phone",
                border: OutlineInputBorder(),
              ),
            ),
            const SizedBox(height: 20),
            TextField(
              controller: passwordController,
              obscureText: true,
              decoration: const InputDecoration(
                labelText: "Password",
                border: OutlineInputBorder(),
              ),
            ),
            const SizedBox(height: 20),
            DropdownButtonFormField<String>(
              value: role,
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
              decoration: const InputDecoration(
                border: OutlineInputBorder(),
                labelText: "Role",
              ),
            ),
            const SizedBox(height: 30),
            loading
                ? const CircularProgressIndicator()
                : SizedBox(
                    width: double.infinity,
                    child: ElevatedButton(
                      onPressed: register,
                      child: const Padding(
                        padding: EdgeInsets.all(15),
                        child: Text("REGISTER"),
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
  const CustomerHomePage({
    super.key,
  });

  @override
  State<CustomerHomePage> createState() => _CustomerHomePageState();
}

class _CustomerHomePageState extends State<CustomerHomePage> {
  String search = "";

  @override
  Widget build(BuildContext context) {
    User user = FirebaseAuth.instance.currentUser!;

    return Scaffold(
      appBar: AppBar(
        title: const Text("Foods"),
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
                if (!snapshot.hasData) {
                  return const Center(
                    child: CircularProgressIndicator(),
                  );
                }

                var products = snapshot.data!.docs;
                products = products.where((e) {
                  var data = e.data() as Map<String, dynamic>;
                  return data["name"]
                      .toString()
                      .toLowerCase()
                      .contains(search);
                }).toList();

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
                    var product = products[index].data() as Map<String, dynamic>;
                    return Card(
                      child: Padding(
                        padding: const EdgeInsets.all(10),
                        child: Column(
                          children: [
                            Expanded(
                              child: Image.network(
                                product["image"],
                                fit: BoxFit.cover,
                                errorBuilder: (context, error, stackTrace) {
                                  return const Icon(
                                    Icons.fastfood,
                                    size: 100,
                                  );
                                },
                              ),
                            ),
                            const SizedBox(height: 10),
                            Text(
                              product["name"],
                              style: const TextStyle(fontWeight: FontWeight.bold),
                            ),
                            Text(
                              "Rs. ${product["price"]}",
                              style: const TextStyle(color: Colors.orange),
                            ),
                            const SizedBox(height: 10),
                            SizedBox(
                              width: double.infinity,
                              child: ElevatedButton(
                                onPressed: () async {
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
                                  } else {
                                    // Add new item
                                    await FirebaseFirestore.instance
                                        .collection("cart")
                                        .add({
                                      "userId": user.uid,
                                      "productId": products[index].id,
                                      "productName": product["name"],
                                      "price": product["price"],
                                      "image": product["image"],
                                      "quantity": 1,
                                    });
                                  }

                                  if (!context.mounted) return;
                                  ScaffoldMessenger.of(context).showSnackBar(
                                    SnackBar(
                                      content: Text(
                                        "${product["name"]} added to cart",
                                      ),
                                      duration: const Duration(seconds: 1),
                                    ),
                                  );
                                },
                                child: const Text("Add to Cart"),
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
// CART SHEET
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

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: MediaQuery.of(context).size.height * 0.9,
      child: StreamBuilder<QuerySnapshot>(
        stream: FirebaseFirestore.instance
            .collection("cart")
            .where("userId", isEqualTo: widget.userId)
            .snapshots(),
        builder: (context, snapshot) {
          if (!snapshot.hasData) {
            return const Center(
              child: CircularProgressIndicator(),
            );
          }

          var cartItems = snapshot.data!.docs;
          double total = 0;

          for (var item in cartItems) {
            var data = item.data() as Map<String, dynamic>;
            total += data["price"] * data["quantity"];
          }

          return Padding(
            padding: const EdgeInsets.all(15),
            child: Column(
              children: [
                const Text(
                  "My Cart",
                  style: TextStyle(
                    fontSize: 25,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: 20),
                Expanded(
                  child: cartItems.isEmpty
                      ? const Center(
                          child: Text("Your cart is empty"),
                        )
                      : ListView.builder(
                          itemCount: cartItems.length,
                          itemBuilder: (context, index) {
                            var data = cartItems[index].data() as Map<String, dynamic>;
                            return Card(
                              child: ListTile(
                                leading: Image.network(
                                  data["image"],
                                  width: 50,
                                  height: 50,
                                  fit: BoxFit.cover,
                                ),
                                title: Text(data["productName"]),
                                subtitle: Text("Rs. ${data["price"]}"),
                                trailing: Row(
                                  mainAxisSize: MainAxisSize.min,
                                  children: [
                                    IconButton(
                                      icon: const Icon(Icons.remove),
                                      onPressed: () async {
                                        if (data["quantity"] > 1) {
                                          await cartItems[index].reference.update({
                                            "quantity": FieldValue.increment(-1)
                                          });
                                        } else {
                                          await cartItems[index].reference.delete();
                                        }
                                      },
                                    ),
                                    Text(
                                      "${data["quantity"]}",
                                      style: const TextStyle(fontSize: 16),
                                    ),
                                    IconButton(
                                      icon: const Icon(Icons.add),
                                      onPressed: () async {
                                        await cartItems[index].reference.update({
                                          "quantity": FieldValue.increment(1)
                                        });
                                      },
                                    ),
                                  ],
                                ),
                              ),
                            );
                          },
                        ),
                ),
                if (cartItems.isNotEmpty) ...[
                  TextField(
                    controller: addressController,
                    decoration: const InputDecoration(
                      labelText: "Delivery Address",
                      border: OutlineInputBorder(),
                    ),
                    maxLines: 2,
                  ),
                  const SizedBox(height: 20),
                  Text(
                    "Total: Rs. ${total.toStringAsFixed(2)}",
                    style: const TextStyle(
                      fontSize: 22,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 20),
                  SizedBox(
                    width: double.infinity,
                    child: ElevatedButton(
                      onPressed: () async {
                        if (addressController.text.isEmpty) {
                          ScaffoldMessenger.of(context).showSnackBar(
                            const SnackBar(
                              content: Text("Please enter delivery address"),
                            ),
                          );
                          return;
                        }

                        List<Map<String, dynamic>> items = [];
                        for (var item in cartItems) {
                          var data = item.data() as Map<String, dynamic>;
                          items.add({
                            "productId": data["productId"],
                            "productName": data["productName"],
                            "price": data["price"],
                            "quantity": data["quantity"],
                            "image": data["image"],
                          });
                        }

                        await FirebaseFirestore.instance
                            .collection("orders")
                            .add({
                          "customerId": widget.userId,
                          "items": items,
                          "total": total,
                          "address": addressController.text,
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

                        if (!context.mounted) return;
                        Navigator.pop(context);
                        ScaffoldMessenger.of(context).showSnackBar(
                          const SnackBar(
                            content: Text("Order Placed Successfully!"),
                            backgroundColor: Colors.green,
                          ),
                        );
                      },
                      child: const Padding(
                        padding: EdgeInsets.all(15),
                        child: Text("PLACE ORDER"),
                      ),
                    ),
                  ),
                ],
              ],
            ),
          );
        },
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
      ),
      body: StreamBuilder<QuerySnapshot>(
        stream: FirebaseFirestore.instance
            .collection("orders")
            .where("customerId", isEqualTo: userId)
            .orderBy("createdAt", descending: true)
            .snapshots(),
        builder: (context, snapshot) {
          if (!snapshot.hasData) {
            return const Center(
              child: CircularProgressIndicator(),
            );
          }

          var orders = snapshot.data!.docs;

          if (orders.isEmpty) {
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
                ],
              ),
            );
          }

          return ListView.builder(
            itemCount: orders.length,
            itemBuilder: (context, index) {
              var order = orders[index].data() as Map<String, dynamic>;
              return Card(
                margin: const EdgeInsets.all(10),
                child: ExpansionTile(
                  leading: CircleAvatar(
                    backgroundColor: getStatusColor(order["status"]),
                    child: Text(
                      order["status"][0],
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
                      Text("Total: Rs. ${order["total"]}"),
                      const SizedBox(height: 5),
                      Container(
                        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                        decoration: BoxDecoration(
                          color: getStatusColor(order["status"]).withOpacity(0.1),
                          borderRadius: BorderRadius.circular(10),
                        ),
                        child: Text(
                          order["status"],
                          style: TextStyle(
                            color: getStatusColor(order["status"]),
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
                            style: TextStyle(fontWeight: FontWeight.bold),
                          ),
                          const SizedBox(height: 8),
                          ...List.generate(
                            (order["items"] as List).length,
                            (i) {
                              var item = order["items"][i];
                              return Padding(
                                padding: const EdgeInsets.symmetric(vertical: 4),
                                child: Row(
                                  children: [
                                    Expanded(
                                      child: Text(item["productName"]),
                                    ),
                                    Text("x${item["quantity"]}"),
                                    const SizedBox(width: 10),
                                    Text(
                                      "Rs. ${item["price"] * item["quantity"]}",
                                      style: const TextStyle(fontWeight: FontWeight.bold),
                                    ),
                                  ],
                                ),
                              );
                            },
                          ),
                          const Divider(),
                          Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              const Text("Delivery Address:"),
                              Expanded(
                                child: Text(
                                  order["address"],
                                  textAlign: TextAlign.right,
                                  style: const TextStyle(fontSize: 12),
                                ),
                              ),
                            ],
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
// ORDER TRACKING (UPDATED WITH NEW STATUSES)
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
      ),
      body: StreamBuilder<DocumentSnapshot>(
        stream: FirebaseFirestore.instance
            .collection("orders")
            .doc(orderId)
            .snapshots(),
        builder: (context, snapshot) {
          if (!snapshot.hasData) {
            return const Center(
              child: CircularProgressIndicator(),
            );
          }

          var order = snapshot.data!.data() as Map<String, dynamic>;
          String status = order["status"];

          return Column(
            children: [
              Expanded(
                child: Stepper(
                  currentStep: getStep(status),
                  controlsBuilder: (_, __) => const SizedBox(),
                  steps: const [
                    Step(
                      title: Text("Pending"),
                      content: Text("Your order has been placed and waiting for confirmation"),
                      isActive: true,
                    ),
                    Step(
                      title: Text("Preparing"),
                      content: Text("Restaurant is preparing your food"),
                      isActive: true,
                    ),
                    Step(
                      title: Text("Delivering"),
                      content: Text("Rider is on the way to deliver your order"),
                      isActive: true,
                    ),
                    Step(
                      title: Text("Delivered"),
                      content: Text("Your order has been delivered successfully"),
                      isActive: true,
                    ),
                  ],
                ),
              ),
              if (status != "Delivered")
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
                        size: 30,
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
                                fontSize: 16,
                              ),
                            ),
                            if (status == "Delivering")
                              const Text(
                                "Your rider is on the way!",
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
// RIDER HOME (UPDATED WITH NEW STATUSES)
// ==========================================================

class RiderHomePage extends StatelessWidget {
  const RiderHomePage({
    super.key,
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
    User rider = FirebaseAuth.instance.currentUser!;

    return Scaffold(
      appBar: AppBar(
        title: const Text("Rider Dashboard"),
        actions: [
          IconButton(
            onPressed: () async {
              await FirebaseAuth.instance.signOut();
            },
            icon: const Icon(Icons.logout),
          ),
        ],
      ),
      body: DefaultTabController(
        length: 2,
        child: Column(
          children: [
            const TabBar(
              tabs: [
                Tab(text: "Active Orders"),
                Tab(text: "Completed"),
              ],
            ),
            Expanded(
              child: TabBarView(
                children: [
                  // Active Orders Tab
                  StreamBuilder<QuerySnapshot>(
                    stream: FirebaseFirestore.instance
                        .collection("orders")
                        .where("status", whereIn: ["Pending", "Preparing", "Delivering"])
                        .orderBy("createdAt", descending: true)
                        .snapshots(),
                    builder: (context, snapshot) {
                      if (!snapshot.hasData) {
                        return const Center(child: CircularProgressIndicator());
                      }

                      var orders = snapshot.data!.docs;
                      
                      if (orders.isEmpty) {
                        return const Center(
                          child: Text("No active orders"),
                        );
                      }

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
                      if (!snapshot.hasData) {
                        return const Center(child: CircularProgressIndicator());
                      }

                      var orders = snapshot.data!.docs;
                      
                      if (orders.isEmpty) {
                        return const Center(
                          child: Text("No completed orders"),
                        );
                      }

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
            ),
          ],
        ),
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
                    color: getStatusColor(order["status"]).withOpacity(0.1),
                    borderRadius: BorderRadius.circular(10),
                  ),
                  child: Text(
                    order["status"],
                    style: TextStyle(
                      color: getStatusColor(order["status"]),
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 10),
            Text(
              "Total: Rs. ${order["total"]}",
              style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 5),
            Row(
              children: [
                const Icon(Icons.location_on, size: 16, color: Colors.grey),
                const SizedBox(width: 5),
                Expanded(
                  child: Text(
                    order["address"],
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
              (order["items"] as List).length,
              (i) {
                var item = order["items"][i];
                return Padding(
                  padding: const EdgeInsets.only(left: 10, top: 5),
                  child: Text("• ${item["productName"]} x${item["quantity"]}"),
                );
              },
            ),
            if (showActions) ...[
              const SizedBox(height: 15),
              // Action buttons based on status
              if (order["status"] == "Pending")
                SizedBox(
                  width: double.infinity,
                  child: ElevatedButton(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.green,
                    ),
                    onPressed: () async {
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
                    },
                    child: const Text("ACCEPT ORDER"),
                  ),
                ),
              
              if (order["status"] == "Preparing" && order["riderId"] == riderId)
                SizedBox(
                  width: double.infinity,
                  child: ElevatedButton(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.orange,
                    ),
                    onPressed: () async {
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
                    },
                    child: const Text("START DELIVERY"),
                  ),
                ),
              
              if (order["status"] == "Delivering" && order["riderId"] == riderId)
                SizedBox(
                  width: double.infinity,
                  child: ElevatedButton(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.blue,
                    ),
                    onPressed: () async {
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