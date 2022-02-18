import Page, { getServerSideProps } from './index.page';

const props: any = {
    id: 'mockId',
};

describe('Log out page', () => {

    it('gets required server side props', async () => {

        const serverSideProps = await getServerSideProps({
            req: {
                cookie: 'mockCookie=value'
            }
        } as any);
                
        expect(serverSideProps).toHaveProperty('redirect');

    });
    
});
