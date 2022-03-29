import App from 'next/app';
import { useRouter } from 'next/router';
import ErrorPage from '@pages/500.page';

import { StandardLayout } from '@components/_pageLayouts/StandardLayout';
import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { AdminLayout } from '@components/_pageLayouts/AdminLayout';
import { layoutIds } from '@constants/routes';

import '../UI/scss/screen.scss';

const CustomApp = ({ Component, pageProps }) => {

    const router = useRouter();
    const { errors, layoutId } = pageProps;

    const hasServerError: boolean = errors?.filter(error => Object.keys(error).filter(key => Number(key) >= 500)).length > 0;

    if(hasServerError){

        return (
        
            <StandardLayout {...pageProps} user={null}>
                <ErrorPage statusCode={500} />
            </StandardLayout>

        )

    }

    if(layoutId === layoutIds.GROUP){

        return (
        
            <GroupLayout {...pageProps}>
                <Component {...pageProps} key={router.asPath} />
            </GroupLayout>

        )

    }

    if(layoutId === layoutIds.ADMIN){

        return (
        
            <AdminLayout {...pageProps}>
                <Component {...pageProps} key={router.asPath} />
            </AdminLayout>

        )

    }

    return (
    
        <StandardLayout {...pageProps}>
            <Component {...pageProps} key={router.asPath} />
        </StandardLayout>

    )
    
}

CustomApp.getInitialProps = async (appContext) => {
      
    const appProps = await App.getInitialProps(appContext);
    
    return { ...appProps }

}

export default CustomApp;
