import { createTheme, responsiveFontSizes } from "@mui/material/styles";

// Color definitions
const primaryLight = "#ccf"; // Lighter shade
const primary = "#aaf"; // Main color from your button
const primaryDark = "#88d"; // Darker shade
const secondaryLight = "#eef"; // From your button border
const secondary = "#ddf";
const secondaryDark = "#cce";
const white = "#fff"; // From your button text
const black = "#333"; // For regular text

// Create the theme
let theme = createTheme({
  palette: {
    primary: {
      light: primaryLight,
      main: primary,
      dark: primaryDark,
      contrastText: white,
    },
    secondary: {
      light: secondaryLight,
      main: secondary,
      dark: secondaryDark,
      contrastText: black,
    },
    background: {
      default: white,
      paper: "#fafafa",
    },
    text: {
      primary: black,
      secondary: "#555",
    },
  },
  typography: {
    fontFamily: '"Roboto", "Helvetica", "Arial", sans-serif',
    button: {
      fontWeight: 700, // From your button
      textTransform: "none", // Optional: prevents all-caps buttons
    },
  },
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 6, // From your button
          padding: "8px 16px",
        },
        contained: {
          border: `1px solid ${secondaryLight}`, // From your button
          "&:hover": {
            backgroundColor: primaryLight,
          },
        },
      },
      defaultProps: {
        variant: "contained", // Default to contained buttons
        disableElevation: true, // Flat buttons without shadow
      },
    },
  },
});

// Make typography responsive
theme = responsiveFontSizes(theme);

export default theme;
