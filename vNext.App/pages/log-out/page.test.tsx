import * as React from 'react';
import { render, screen } from '@testing-library/react';

import Page, { getServerSideProps } from './index.page';

import { Props } from '@components/_pageTemplates/LoggedOutTemplate/interfaces';

const props: Props = {
    id: 'mockId',
    text: {
        title: 'mockTitle',
        metaDescription: 'mockMetaDescriptionText',
        mainHeading: 'mockMainHeading',
    },
    logOutUrl: '/mock'
};

describe('Log out page', () => {

    it('renders correctly', () => {

        render(<Page {...props} />);

        expect(screen.getAllByText('mockMainHeading').length).toEqual(1);

    });

    it('gets required server side props', async () => {

        const serverSideProps = await getServerSideProps({
            req: {
                cookie: 'mockCookie=value'
            }
        } as any);
                
        expect(serverSideProps).toHaveProperty('props.text');

    });
    
});
