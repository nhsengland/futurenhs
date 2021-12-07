import { withApplicationInsights } from 'next-applicationinsights';
import '../UI/scss/screen.scss';

const App = ({ Component, pageProps }) => <Component {...pageProps} />

export default withApplicationInsights({ 
    instrumentationKey: process.env.APPINSIGHTS_INSTRUMENTATIONKEY,
    isEnabled: process.env.APPINSIGHTS_INSTRUMENTATIONKEY && process.env.NODE_ENV === 'production'
})(App as any);
