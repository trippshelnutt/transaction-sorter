import Vue from 'vue'
import axios from 'axios'

interface ITransaction {
    amount:string,
    payee:string,
}

interface ITransactionData {
    DisplayAmount:string,
    payee_name:string,
}

var app = new Vue({
    el: '#app',
    data: {
        title: 'Transaction Sorter',
        transactions: [] as ITransaction[],
        categories: [
            { text: 'Allowance - Missy', value: 'Missy' },
            { text: 'Allowance - Tripp', value: 'Tripp' },
            { text: 'Spending - Dining Out', value: 'DiningOut' },
            { text: 'Spending - Groceries', value: 'Groceries' },
            { text: 'Spending - Supplies', value: 'Supplies' },
        ],
        selectedCategory: 'Missy',
        months: [
            { text: '01 - Jan', value: '1' },
            { text: '02 - Feb', value: '2' },
            { text: '03 - Mar', value: '3' },
            { text: '04 - Apr', value: '4' },
            { text: '05 - May', value: '5' },
            { text: '06 - Jun', value: '6' },
            { text: '07 - Jul', value: '7' },
            { text: '08 - Aug', value: '8' },
            { text: '09 - Sep', value: '9' },
            { text: '10 - Oct', value: '10' },
            { text: '11 - Nov', value: '11' },
            { text: '12 - Dec', value: '12' },
        ],
        selectedMonth: '1'
    },
    mounted () {
        //this.getTransactions()
    },
    methods: {
        getTransactions: function() {
            axios
                .get(`/api/transactions/${ this.selectedMonth }/${ this.selectedCategory}`)
                .then(response => {
                    const transactions = response.data.data.transactions as Array<ITransactionData>;
                    console.debug(transactions);
                    this.transactions = transactions.map((t: { DisplayAmount:string, payee_name:string }) : ITransaction => {
                        return { amount: t.DisplayAmount, payee: t.payee_name };
                    });
                });
        }
    }
});
