/** @type {import('tailwindcss').Config} */
export default {
    content: ["./Pages/**/*.{html,js,razor}", "./Shared/**/*.{html,js,razor}"],
    theme: {
        colors: {

            "white": "#ffffff",
            "black": "#000000",
            "gray": "#808080",
            "blue": "#0000ff",
            "red": "#ff0000",
            "green": "#008000",
            "yellow": "#ffff00",
            "cyan": "#00ffff",
            "magenta": "#ff00ff",
            "orange": "#ffa500",
            "purple": "#800080",
            "pink": "#ffc0cb",
            "brown": "#a52a2a",
            "lightgray": "#d3d3d3",
            "darkgray": "#a9a9a9"
                
        }
        extend: {},
    },
    plugins: [],
}