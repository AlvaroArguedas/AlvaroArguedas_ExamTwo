<template>
    <div class="d-flex vh-100 overflow-hidden">
        <div class="flex-grow-1 p-md-4 overflow-auto" style="background-color: #F5EDE0;">
            <h2 class="mb-3 text-center"> Máquina de Cafés </h2>

            <table class="table  table-bordered">
                <thead class="table-dark">
                    <tr>
                        <th>Producto</th>
                        <th>Disponibles</th>
                        <th>Precio </th>
                        <th>Cantidad</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="coffee in coffeeList" :key="coffee.name">
                        <td>{{ coffee.name }}</td>
                        <td>{{ coffee.quantity }}</td>
                        <td>₡{{ formatColones(coffee.price) }}</td>
                        <td>
                            <input type="number" class="form-control"
                                   min="0" :max="coffee.quantity"
                                   v-model.number="orderItems[coffee.name]"
                                   @input="calculateTotal"
                                   :disabled="coffee.quantity === 0" />
                        </td>
                    </tr>
                </tbody>
            </table>
            <h4 class="text-end mt-3">Total a pagar: <span class="fw-bold">₡{{ formatColones(totalToPay) }}</span></h4>
            <div class="text-center mt-2">
                <img src="@/assets/coffee-cup.png" alt="Coffee cup" style="width: 120px;">
            </div>
        </div>

        <div class="p-4 payment-column">
            <div>
                <h3 class="mb-2 text-center"> Pago</h3>

                <p><strong>Monedas:</strong></p>
                <div class="d-flex mb-2">
                    <button v-for="coin in availableCoins" :key="coin" class="btn btn-outline-primary m-1"
                            @click="addCoin(coin)">
                        ₡{{ formatColones(coin) }}
                    </button>
                </div>

                <p class="mt-2"><strong>Billetes:</strong></p>
                <div class="d-flex flex-wrap mb-2">
                    <button v-for="bill in availableBills" :key="bill" class="btn btn-outline-success m-1"
                            @click="addBill(bill)">
                        ₡{{ formatColones(bill) }}
                    </button>
                </div>

                <p class="mt-2">Pagado: <strong> ₡{{ formatColones(totalPaid) }}</strong></p>

                <button class="btn btn-success w-100 mt-3 mb-5" :disabled="totalPaid === 0" @click="buyOrder">
                    Comprar
                </button>
            </div>

            <div v-if="result" class="result-container ">
                <div :class="result.success ? 'alert alert-success' : 'alert alert-danger'">
                    <div v-if="result.success">
                        Su vuelto es de <strong>₡{{ formatColones(result.totalChange) }}</strong>.<br>
                        <strong>Desglose:</strong>
                        <ul class="mb-2">
                            <li v-for="(qty, coin) in result.changeBreakdown" :key="coin">
                                {{ qty }} moneda(s) de ₡{{ formatColones(coin) }}
                            </li>
                        </ul>
                    </div>
                    <div v-else>
                        {{ result.message }}
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import { getCoffees, getPrices, buyCoffee } from "@/services/coffeeService";

    export default {
        data() {
            return {
                coffeeList: [],
                orderItems: {},
                availableCoins: [25, 50, 100, 500],
                availableBills: [1000, 2000],
                insertedCoins: [],
                insertedBills: [],
                result: null,
                totalToPay: 0
            };
        },
        computed: {
            totalPaid() {
                return [...this.insertedCoins, ...this.insertedBills].reduce((a, b) => a + b, 0);
            }
        },
        async mounted() {
            await this.loadCoffees();
        },
        methods: {
            async loadCoffees() {
                try {
                    const coffeeRes = await getCoffees();
                    const priceRes = await getPrices();
                    this.coffeeList = Object.keys(coffeeRes.data).map(name => ({
                        name,
                        quantity: coffeeRes.data[name],
                        price: priceRes.data[name] || 0
                    }));
                    this.coffeeList.forEach(c => this.orderItems[c.name] = 0);
                } catch (err) {
                    console.error(err);
                }
            },
            formatColones(amount) {
                return amount.toLocaleString("es-CR");
            },
            calculateTotal() {
                this.totalToPay = Object.keys(this.orderItems)
                    .reduce((sum, name) => {
                        const price = this.coffeeList.find(c => c.name === name)?.price || 0;
                        return sum + (this.orderItems[name] * price);
                    }, 0);
            },
            addCoin(value) {
                this.insertedCoins.push(value);
            },
            addBill(value) {
                this.insertedBills.push(value);
            },
            async buyOrder() {
                const items = Object.keys(this.orderItems)
                    .filter(name => this.orderItems[name] > 0)
                    .map(name => ({ Type: name, Quantity: this.orderItems[name] }));

                const payment = {
                    Coins: this.insertedCoins,
                    Bills: this.insertedBills
                };

                try {
                    const res = await buyCoffee({ Items: items, Payment: payment });
                    this.result = res.data;

                    const coffeeRes = await getCoffees();
                    this.coffeeList.forEach(c => c.quantity = coffeeRes.data[c.name]);

                    Object.keys(this.orderItems).forEach(k => this.orderItems[k] = 0);
                    this.insertedCoins = [];
                    this.insertedBills = [];
                    this.totalToPay = 0;

                } catch (err) {
                    this.result = {
                        success: false,
                        message: err.response?.data?.message || "Error al procesar la compra."
                    };
                    this.insertedBills = [];
                    this.insertedCoins = [];
                }
            }
        }
    };
</script>