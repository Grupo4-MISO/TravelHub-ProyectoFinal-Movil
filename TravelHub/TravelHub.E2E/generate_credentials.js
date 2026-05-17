// Generates a random email and password and exposes them via output
const rand = Math.random().toString(36).substring(2, 8);
const email = `test.user.${rand}@example.com`;
const password = `P@ss${Math.floor(Math.random()*9000)+1000}`;

output.email = email;
output.password = password;
console.log('Generated credentials:', email, password);
