import Vue from 'vue'

interface ITransaction {
    data:string
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
    methods: {
        getTransactions: function() {
            this.transactions = [
                { data: this.selectedMonth },
                { data: this.selectedCategory },
                { data: this.selectedMonth },
                { data: this.selectedCategory },
                { data: this.selectedMonth },
            ];
        }
    }
});
