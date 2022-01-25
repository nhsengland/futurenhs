import * as React from 'react';
import { render, screen } from '@testing-library/react';

import Page, { getServerSideProps } from './index.page';
import { Props } from '@components/_pageTemplates/SearchListingTemplate/interfaces';
// TODO
const props: Props = {
    id: "",
    term: "null",
    resultsList: [],
};

describe('Search results', () => {

    it('renders correctly', () => {

        render(<Page {...props} />);

        expect(screen.getAllByText('Searching: null').length).toEqual(1);

    });

    it('gets required server side props', async () => {

        const serverSideProps = await getServerSideProps({} as any);
                
        expect(serverSideProps).toHaveProperty('props.text');

    });
    
});
