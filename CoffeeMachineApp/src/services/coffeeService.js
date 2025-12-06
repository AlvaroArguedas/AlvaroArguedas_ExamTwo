import axios from 'axios'

export const getCoffees = () => axios.get("/getCoffees")
export const getPrices = () => axios.get("/getCoffeePricesInCents")
export const getCoinInventory = () => axios.get("/getQuantityCoins")
export const buyCoffee = (orderRequest) => axios.post("/buyCoffee", orderRequest)
