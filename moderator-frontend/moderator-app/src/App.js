
import React from "react";
import axios from "axios";
import Button from 'react-bootstrap/Button';
import Alert from "react-bootstrap/Alert";
import Form from "react-bootstrap/Form"
import Row from "react-bootstrap/Row"
import Col from "react-bootstrap/Col"

import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';

function App() {
  const [show, setShow] = React.useState(false);
  const [alertVarient, setAlertVarient] = React.useState('');
  const [alerMessage, setAlertMessage] = React.useState('');
  const [showResults, setShowResults] = React.useState(false);
  const [result, setResult] = React.useState();
  const [uploadFile, setUploadFile] = React.useState();
  const [htmlText, setHTMLText] = React.useState();

  const submitForm = e => {
      e.preventDefault();
      setShow(false);
      const formData = new FormData();
      formData.append("htmlText", htmlText);
      formData.append("uploadFile", uploadFile);
      if(uploadFile.size / (1024 * 1024) > 4){
        setAlertVarient('danger');
        setAlertMessage('Please provide image less that 4mb');
        setShow(true);
        return;
      }
      axios
      .post(process.env.REACT_APP_MODERATOR_URL, formData, {
        headers: {
          "Content-Type": "multipart/form-data"
        }
      })
      .then((response) => {
          setResult(response.data);
          setShowResults(true);
          setHTMLText('');
      })
      .catch((error) => {
        setAlertVarient('danger');
        if(error.response && error.response.data)
          setAlertMessage(error.response.data);
        else
        setAlertMessage('Unable to process request');
        setShow(true);
      });
  };

  const Results = () => (
    <div id="results" className="mt-5 search-results">
      <h3>Image Score</h3>
      <div className="row mb-3">
        <Row className="mb-2">
          <Col sm={6} md={3}>Adult Classification Score</Col>
          <Col sm={6} md={3}>{result.imageResult.executionResult.adultClassificationScore}</Col>
        </Row>
        <Row className="mb-2">
          <Col sm={6} md={3}>Is Image Adult Classified</Col>
          <Col sm={6} md={3}>{result.imageResult.executionResult.isImageAdultClassified.toString()}</Col>
        </Row>
        <Row className="mb-2">
          <Col sm={6} md={3}>Racy Classification Score</Col>
          <Col sm={6} md={3}>{result.imageResult.executionResult.racyClassificationScore}</Col>
        </Row>
        <Row className="mb-2">
          <Col sm={6} md={3}>Is Image Racy Classified</Col>
          <Col sm={6} md={3}>{result.imageResult.executionResult.isImageRacyClassified.toString()}</Col>
        </Row>
        <Row className="mb-2">
          <Col sm={6} md={3}>Execution Time in Milliseconds</Col>
          <Col sm={6} md={3}>{result.imageResult.executionTime}</Col>
        </Row>
      </div>
      <h3>Text Score</h3>
      <div className="row">
        <Row className="mb-2">
          <Col sm={6} md={3}>Sexual Explicit or Adult Score</Col>
          <Col sm={6} md={3}>{result.textResult.executionResult.classification.category1.score}</Col>
        </Row>
        <Row className="mb-2">
          <Col sm={6} md={3}>Sexual Suggestive or Mature Score</Col>
          <Col sm={6} md={3}>{result.textResult.executionResult.classification.category2.score}</Col>
        </Row>
        <Row className="mb-2">
          <Col sm={6} md={3}>Offensive Language Score</Col>
          <Col sm={6} md={3}>{result.textResult.executionResult.classification.category3.score}</Col>
        </Row>
        <Row className="mb-2">
          <Col sm={6} md={3}>Review Recommended</Col>
          <Col sm={6} md={3}>{result.textResult.executionResult.classification.reviewRecommended.toString()}</Col>
        </Row>
        <Row className="mb-2">
          <Col sm={6} md={3}>Execution Time in Milliseconds</Col>
          <Col sm={6} md={3}>{result.textResult.executionTime}</Col>
        </Row>
      </div>
    </div>
  )

  return (
      <div fluid="md" className="p-5">
         <Alert show={show} variant={alertVarient}>
            {alerMessage}
        </Alert>
          <div className="row justify-content-md-center">
              <h2>
                  Sparrow Post Moderator
              </h2>
              <Form onSubmit={submitForm}>
                  <Form.Group className="mb-3" controlId="moderator.ImageInput">
                  <Form.Label>Upload Image</Form.Label>
                  <Form.Control required accept="image/*" type="file" onChange={(e) => setUploadFile(e.target.files[0])} />
                  </Form.Group>
                  <Form.Group className="mb-3" controlId="moderator.HtmlTextArea">
                      <Form.Label>Paste HTML String</Form.Label>
                      <Form.Control required as="textarea" value={htmlText} maxLength="1024" rows={5} onChange={(e) => setHTMLText(e.target.value)} />
                  </Form.Group>
                  <Button variant="primary" size="lg" type="submit">
                      Submit
                  </Button>
              </Form>
          </div>

          <div className="mt-3">
          { showResults ? <Results /> : null }
          </div>
      </div>
  );
}
export default App;
