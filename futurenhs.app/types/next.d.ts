import { GetServerSideProps } from 'next';
import { GetServerSidePropsContext as NextGetServerSidePropsContext, GetServerSideProps } from 'next';
import { User } from './user';

export interface GetServerSidePropsContext extends NextGetServerSidePropsContext {
    req: NextGetServerSidePropsContext.req & {
        user?: User;
        body?: any;
    };
    props?: Record<any, any>;
}

export interface HofConfig {
    routeId?: string;
    getServerSideProps: GetServerSideProps;
}